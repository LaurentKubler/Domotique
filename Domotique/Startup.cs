using Domotique.Database;
using Domotique.Model;
using Domotique.Service;
using Domotique.Service.Log;
using Messages.Queue.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Collections.Generic;

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
            services.AddSingleton<IDatabaseConnection>(c => { return new DatabaseConnection(Configuration.GetValue<string>("Services:Database:ConnectionString")); });

            services.AddSingleton<ITemperatureReadingService, TemperatureReadingService>();/* new TemperatureReadingService(new DataRead()
            {
                ServerName = Configuration.GetValue<string>("Services:Temperature:ServerName"),
                ServerPort = Configuration.GetValue<int>("Services:Temperature:ServerPort"),
                QueueName = Configuration.GetValue<string>("Services:Temperature:QueueName")
            });*/
            services.AddDbContext<DomotiqueContext>(
                options => options.UseMySql(Configuration.GetValue<string>("Services:Database:ConnectionString"), // replace with your Connection String
                    mysqlOptions =>
                    {
                        mysqlOptions.ServerVersion(new Version(5, 7, 17), ServerType.MySql); // replace with your Server Version and Type
                    }
            ));
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IDeviceStatusReadingService, DeviceStatusReadingService>();
            services.AddSingleton<IQueueConnectionFactory, QueueConnectionFactory>();

            services.AddSingleton<IDBContextProvider>(c => { return new DBContextProvider(Configuration.GetValue<string>("Services:Database:ConnectionString")); });

            var queues = new List<QueueConfiguration>();
            var nodes = Configuration.GetSection("Services:Queues");

            foreach (var node in nodes.GetChildren())
            {
                queues.Add(new QueueConfiguration(node));
            }
            services.AddSingleton<IQueueConnectionFactory>((c) => new QueueConnectionFactory(queues));


            services.AddSingleton<IDeviceStatusReadingService, DeviceStatusReadingService>();
            services.AddTransient<IDataRead, DataRead>();
            services.AddTransient<IDeviceService, DeviceService>();


            TemperatureReadingService.ServerName = Configuration.GetValue<string>("Services:Temperature:ServerName");
            TemperatureReadingService.QueueName = Configuration.GetValue<string>("Services:Temperature:QueueName");

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

            var tmp = app.ApplicationServices.GetService<ITemperatureReadingService>();
            tmp.Start();
            var tmp2 = app.ApplicationServices.GetService<IDeviceStatusReadingService>();
            tmp2.Start();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSpa(spa =>
            {
                //spa.Options.SourcePath = "C:\\Users\\lkubler\\source\\perso\\Domotique\\Domotique\\ClientApp";
                //spa.Options.SourcePath = "C:\\Users\\Laurent\\Source\\Repos\\Domotique\\Domotique\\ClientApp";
                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
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
