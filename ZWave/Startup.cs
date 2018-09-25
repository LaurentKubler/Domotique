using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZWave.CommandClasses;

namespace ZWave
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

            app.UseHttpsRedirection();
            app.UseMvc();            

            // Register the Controller event handlers (see methods example below)
            var controller = new ZWaveController("COM5");
            controller.Open();
            var nodesTask = controller.GetNodes();
            nodesTask.Wait();
            var nodes = nodesTask.Result;
            foreach (var node in nodes)
            {
                Subscribe(node);
            }
            while (true)
                Thread.Sleep(1000);
        }

        private static void Subscribe(Node node)
        {
            var basic = node.GetCommandClass<Basic>();
            basic.Changed += (_, e) => Console.WriteLine($"Basic report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sensorMultiLevel = node.GetCommandClass<SensorMultiLevel>();
            sensorMultiLevel.Changed += (_, e) => Console.WriteLine($"SensorMultiLevel report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var meter = node.GetCommandClass<Meter>();
            meter.Changed += (_, e) => Console.WriteLine($"Meter report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var alarm = node.GetCommandClass<Alarm>();
            alarm.Changed += (_, e) => Console.WriteLine($"Alarm report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sensorBinary = node.GetCommandClass<SensorBinary>();
            sensorBinary.Changed += (_, e) => Console.WriteLine($"SensorBinary report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sensorAlarm = node.GetCommandClass<SensorAlarm>();
            sensorAlarm.Changed += (_, e) => Console.WriteLine($"SensorAlarm report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var wakeUp = node.GetCommandClass<WakeUp>();
            wakeUp.Changed += (_, e) => { Console.WriteLine($"WakeUp report of Node {e.Report.Node:D3} changed to [{e.Report}]"); };

            var switchBinary = node.GetCommandClass<SwitchBinary>();
            switchBinary.Changed += (_, e) => Console.WriteLine($"SwitchBinary report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var thermostatSetpoint = node.GetCommandClass<ThermostatSetpoint>();
            thermostatSetpoint.Changed += (_, e) => Console.WriteLine($"thermostatSetpoint report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sceneActivation = node.GetCommandClass<SceneActivation>();
            sceneActivation.Changed += (_, e) => Console.WriteLine($"sceneActivation report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var multiChannel = node.GetCommandClass<MultiChannel>();
            multiChannel.Changed += (_, e) => Console.WriteLine($"multichannel report of Node {e.Report.Node:D3} changed to [{e.Report}]");
        }

    }
}
