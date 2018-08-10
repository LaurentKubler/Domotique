using Domotique.Database;
using Domotique.Service;
using Domotique.Service.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique
{
    public partial class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITemperatureReadingService>(new TemperatureReadingService()
            {
                ServerName = Configuration.GetValue<string>("Services:Temperature:ServerName"),
                ServerPort = Configuration.GetValue<int>("Services:Temperature:ServerPort"),
                QueueName = Configuration.GetValue<string>("Services:Temperature:QueueName")
            });
            services.AddDbContext<DomotiqueContext>();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IStatusService, Status>();
            
            if (Configuration.GetValue<bool>("Services:Logger:GlobalLogEnabled"))
            {                
            }
            services.AddMvc();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Start the main server
            var tmp = app.ApplicationServices.GetService<IStatusService>();

            app.UseMvc();

/*
            var factory = new ConnectionFactory() { HostName = "localhost" };
            string queueName = "hello";
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = "testemessasssge";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            } */
        }
    }
}
