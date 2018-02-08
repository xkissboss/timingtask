using Dy.TaskCache;
using Dy.TaskUtility.Models;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskClient
{
    class Program
    {
        static ConnectionFactory rabbitMQFactory;
        static StdSchedulerFactory schedulerFactory;
        static ICache cache;
        static string key;
        static string exchange;
        static IScheduler sched;


        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            string filePath = File.Exists("/etc/webconfig/task_clent/appsettings.json") ? "/etc/webconfig/task_clent/appsettings.json" : "appsettings.json";
            builder.AddJsonFile(filePath);
            var configuration = builder.Build();

            IConfigurationSection rabbitSection = configuration.GetSection("ConnectionStrings").GetSection("RabbitMQ");

            rabbitMQFactory = new ConnectionFactory()
            {
                HostName = rabbitSection["HostName"],
                UserName = rabbitSection["UserName"],
                Password = rabbitSection["Password"]
            };
            key = args.Length > 0 ? args[0] : rabbitSection["Key"];

            Console.WriteLine($"key:{key}");

            exchange = rabbitSection["Exchange"];


            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            schedulerFactory = new StdSchedulerFactory(props);

            IConfigurationSection redisSection = configuration.GetSection("ConnectionStrings").GetSection("Redis");
            cache = new RedisCache(redisSection.GetValue<string>("ConnString"), redisSection.GetValue<int>("RedisDbNumber"), redisSection.GetValue<string>("Prefix"), redisSection.GetValue<int>("ExpireSeconds"));
            InitTask().Wait();
            RabbitMQTask();
            Console.WriteLine("Hello World!");
        }

        static void RabbitMQTask()
        {
            using (var connection = rabbitMQFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);

                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queue: queueName,
                                      exchange: exchange,
                                      routingKey: key);
                    Console.WriteLine($"queueName========》{queueName}");
                    Console.WriteLine(" [*] Waiting for logs.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        Console.WriteLine(" [x] {0}", message);

                        TaskModel[] modelArr = cache.GetList<TaskModel>("task_list", delegate() { return null; });
                        if(modelArr != null)
                        {
                            TaskModel taskModel = modelArr.FirstOrDefault(d => d.TaskID.ToString() == message);
                            if (taskModel != null)
                            {
                                Console.WriteLine($"{taskModel.TaskName} --- {taskModel.TaskDesc}");
                                CreateJob(taskModel);
                            }
                        }

                        channel.BasicAck(ea.DeliveryTag, false);
                    };
                    channel.BasicConsume(queue: queueName,
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }


        static async Task InitTask()
        {
            sched = await schedulerFactory.GetScheduler();
            await sched.Start();
            TaskModel[] modelArr = cache.GetList<TaskModel>("task_list", delegate () { return null; });
            if (modelArr != null)
            {
                TaskModel[] taskArr = modelArr.Where(d => d.IsActive == true && d.TaskKey == key).ToArray();
                foreach(TaskModel model in taskArr)
                {
                    CreateJob(model);
                }
            }
        }


        static async void CreateJob(TaskModel taskModel)
        {
            string taskId = taskModel.TaskID.ToString();
            if (await sched.CheckExists(new JobKey(taskId, taskId)))
                await sched.DeleteJob(new JobKey(taskId, taskId));

            // 禁止则返回
            if (!taskModel.IsActive)
            {
                return;
            }

            IJobDetail job = (taskModel.IsDce ? JobBuilder.Create<DCETaskJob>(): JobBuilder.Create<TaskJob>())
                .WithIdentity(taskId, taskId)
                .UsingJobData("url", taskModel.TaskUrl)
                .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(taskId, taskId)
                .WithCronSchedule(taskModel.CronExpressionString)
                .ForJob(taskId, taskId)
                .Build();

            
            await sched.ScheduleJob(job, trigger);
        }
    }
}
