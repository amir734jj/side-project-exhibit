using System;
using System.IO;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var portCandidate = Environment.GetEnvironmentVariable("PORT");
            var port = string.IsNullOrWhiteSpace(portCandidate) ? portCandidate : "5000";
            
            var host = WebHost.CreateDefaultBuilder(args)
                .UseLamar()
                .UseUrls($"http://*:{port}")
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Error))
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                })
                .Build();
            
            host.Run();
        }
    }
}