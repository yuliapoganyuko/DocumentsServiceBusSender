using DocumentsServiceBusSender.Core;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace DocumentsServiceBusSender
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureAppConfiguration(config => 
                    config
                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables())
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ServiceBusService>();
                    services.AddScoped<JsonHandler>();
                })
                .Build();

            host.Run();
        }
    }
}