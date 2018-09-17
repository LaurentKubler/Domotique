using Domotique.Database;
using Domotique.Model;
using Domotique.Service;
using Domotique.Service.Log;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.Builder;
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
            services.AddSingleton<ITemperatureReadingService>(new TemperatureReadingService(new DataRead())
            {
                ServerName = Configuration.GetValue<string>("Services:Temperature:ServerName"),
                ServerPort = Configuration.GetValue<int>("Services:Temperature:ServerPort"),
                QueueName = Configuration.GetValue<string>("Services:Temperature:QueueName")
            });
            services.AddDbContext<DomotiqueContext>();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IStatusService, Status>();
            services.AddTransient<IDatabaseConnection>(c => { return new DatabaseConnection(Configuration.GetValue<string>("Services:Database:ConnectionString")); });
            services.AddTransient<IDataRead, DataRead>();
            
            //services.AddWebSocketManager();

            if (Configuration.GetValue<bool>("Services:Logger:GlobalLogEnabled"))
            {                
            }
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddMvc();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Start the main server
            var tmp = app.ApplicationServices.GetService<IStatusService>();
         /*   command.CommandText = "select  RoomId, Name, min(CurrentTemp) Min, max(CurrentTemp) Max,DATE(LogDate) " +
                "from TemperatureLog " +
                "inner join Room on Room.ID = TemperatureLog.RoomId " +
                "where  LogDate >  date_sub(now(),INTERVAL 1 WEEK) group by  RoomId, DATE(LogDate);";*/            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "C:\\Users\\lkubler\\source\\perso\\Domotique\\Domotique\\ClientApp";                
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
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
    }/*  /*
        select 
            RoomId, Name, min(CurrentTemp) Min, max(CurrentTemp) Max,DATE(LogDate)
        from TemperatureLog
        inner join Room
            on Room.ID = TemperatureLog.RoomId
        where 
            LogDate >  date_sub(now(),INTERVAL 1 WEEK)
        group by 
            RoomId, DATE(LogDate);

    select   
    	l.CurrentTemp,   l.RoomId, l.LogDate
    from 
	    TemperatureLog l   
    where   
	    row(l.LogDate,l.roomId)  
	    in   (
		    select max(LogDate), RoomId 
		    from TemperatureLog 
		    group by RoomId);

  */
}
