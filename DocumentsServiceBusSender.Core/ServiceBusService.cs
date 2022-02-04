using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DocumentsServiceBusSender.Core
{
    public class ServiceBusService: IDisposable
    {
        private ServiceBusClient client;

        private ServiceBusSender sender;

        private ILogger logger;

        public ServiceBusService(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            this.logger = loggerFactory.CreateLogger<ServiceBusService>();
            var connectionString = configuration.GetSection("Values")["AzureServiceBus"];
            string topic = configuration.GetSection("Values")["AzureServiceBusTopicName"];
            client = new ServiceBusClient(connectionString);
            sender = client.CreateSender(topic);
            logger.LogInformation("SBS is created");
        }

        public async Task SendMessage(string messageBody)
        {
            var message = new ServiceBusMessage(messageBody);
            await sender.SendMessageAsync(message);
        }

        public async Task SendMessageWithProperty(string messageBody, string propertyKey, string propertyValue)
        {
            var message = new ServiceBusMessage(messageBody);
            message.ApplicationProperties.Add(propertyKey, propertyValue);
            await sender.SendMessageAsync(message);
        }

        public void Dispose()
        {
            logger.LogInformation("ServiceBusService disposed asyncronously");
            DisposeAsyncCore().ConfigureAwait(false);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (sender is IDisposable)
            {
                logger.LogInformation("sender is IDisposable");
            }

            if (sender is IAsyncDisposable)
            {
                logger.LogInformation("sender is IAsyncDisposable");
            }

            if (sender is not null)
            {
                await sender.DisposeAsync().ConfigureAwait(false);
            }

            if (client is not null)
            {
                await client.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
