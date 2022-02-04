using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DocumentsServiceBusSender.Core
{
    public class ServiceBusService: IAsyncDisposable
    {
        private ServiceBusClient client;

        private ServiceBusSender sender;

        private ILogger logger;

        public ServiceBusService(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            this.logger = loggerFactory.CreateLogger<ServiceBusService>();
            var connectionString = configuration["AzureServiceBus"];
            string topic = configuration["AzureServiceBusTopicName"];
            client = new ServiceBusClient(connectionString);
            sender = client.CreateSender(topic);
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

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
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
