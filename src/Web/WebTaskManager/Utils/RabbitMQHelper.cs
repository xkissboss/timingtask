using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTaskManager.Utils
{
    public class RabbitMQHelper
    {
        public static void PublishMessage(ConnectionFactory factory, string exchange, string key, string message)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: exchange,
                                  routingKey: key,
                                  basicProperties: null,
                                  body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }
        }
    }
}
