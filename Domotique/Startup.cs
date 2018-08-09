using Domotique.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton<TemperatureReadingService>(new TemperatureReadingService()
            {
                ServerName = Configuration.GetValue<string>("Services:Temperature:ServerName"),
                ServerPort = Configuration.GetValue<int>("Services:Temperature:ServerPort"),
                QueueName = Configuration.GetValue<string>("Services:Temperature:QueueName")
            });
            services.AddSingleton<Status>();
            if (Configuration.GetValue<bool>("Services:Logger:GlobalLogEnabled"))
            {                
            }
            services.AddMvc();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.ApplicationServices.GetService<Status>();
            app.UseMvc();

        }
    }
}
