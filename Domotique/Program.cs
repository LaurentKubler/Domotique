using Gelf.Extensions.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using
Microsoft.Extensions.Logging;
using System;

namespace Domotique
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            .ConfigureLogging((context, builder) =>
            {
                // Read GelfLoggerOptions from appsettings.json
                builder.Services.Configure<GelfLoggerOptions>(context.Configuration.GetSection("Graylog"));

                // Optionally configure GelfLoggerOptions further.
                builder.Services.PostConfigure<GelfLoggerOptions>(options =>
                    options.AdditionalFields["machine_name"] = Environment.MachineName);

                // Read Logging settings from appsettings.json and add providers.
                builder.AddConfiguration(context.Configuration.GetSection("Logging"))
                    .AddConsole()
                    .AddDebug()
                    .AddGelf();
            })
                .Build();
    }
}
