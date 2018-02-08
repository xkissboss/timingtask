using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Dy.TaskDB;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using WebTaskManager.Utils;
using Dy.TaskCache;

namespace WebTaskManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContextPool<TaskDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySql")));

            IConfigurationSection rabbitSection = Configuration.GetSection("ConnectionStrings").GetSection("RabbitMQ");
            services.AddSingleton<ConnectionFactory>(sp =>
            {
                var factory = new ConnectionFactory()
                {
                    HostName = rabbitSection["HostName"],
                    Password = rabbitSection["Password"],
                    UserName = rabbitSection["UserName"]
                };
                return factory;
            });

            GlobalVariable.EXCHANGE = rabbitSection["Exchange"];


            services.AddSingleton<ICache>(sp =>
            {
                IConfigurationSection redisSection = Configuration.GetSection("ConnectionStrings").GetSection("Redis");
                return new RedisCache(redisSection.GetValue<string>("ConnString"), redisSection.GetValue<int>("RedisDbNumber"), redisSection.GetValue<string>("Prefix"), redisSection.GetValue<int>("ExpireSeconds"));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, TaskDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            InitDb(dbContext);
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }



        private void InitDb(TaskDbContext dbContext)
        {
            dbContext.Database.Migrate();
        }
    }
}
