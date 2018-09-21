using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PLCBus.Services;

namespace PLCBus
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddTransient<IMessageQueue>((c) =>
            {
                MessageQueue messageQueue = new MessageQueue(Configuration.GetValue<string>("Services:MessageQueue:Server"),
                                                            Configuration.GetValue<string>("Services:MessageQueue:CommandExchange"),
                                                            Configuration.GetValue<string>("Services:MessageQueue:CommandFilter"),
                                                            Configuration.GetValue<string>("Services:MessageQueue:ResponseExchange"),
                                                            Configuration.GetValue<string>("Services:MessageQueue:ResponseTag"));
                return messageQueue;
            });

            services.AddSingleton<IPLCBusService, PLCBusService>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.ApplicationServices.GetService<IPLCBusService>();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
