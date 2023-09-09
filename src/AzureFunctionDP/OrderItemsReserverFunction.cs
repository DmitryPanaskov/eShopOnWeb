using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace OrderItemsReserver
{
    public class OrderItemReserver
    {
        private const string connectionStringBlob = "DefaultEndpointsProtocol=https;AccountName=eshoponwebstorage2023;AccountKey=HKT/lTQIWK7kRnYiJNRSvfrzhF6OWn9L6sOYuTR7ptx/Of/8pFJoJMqksfVvJ1aaGtn7uMIZIQjb+ASt0ewMtA==;EndpointSuffix=core.windows.net";
        //private const string connectionServiceBus = "Endpoint=sb://eshoponweb-dp.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ZRBXlncaNrntMLGYByRxX8iiKGeY1qNg++ASbGwTY8g=";
        [FunctionName("OrderItemReserver")]
        public async Task Run([ServiceBusTrigger("service-bus-queues-dp", Connection = "Endpoint=sb://eshoponweb-dp.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ZRBXlncaNrntMLGYByRxX8iiKGeY1qNg++ASbGwTY8g=")]
            string myQueueItem, ILogger log)
        {
            try
            {
                Console.WriteLine("Start");
                log.LogInformation("Start DeliveryOrderProcessor");

                var blobServiceClient = new BlobServiceClient(connectionStringBlob);
                var containerName = "orderitems";
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var fileName = "item_" + Guid.NewGuid().ToString() + ".json";
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(myQueueItem);

                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
            }
        }
    }
}


/*

        private static async Task WriteToAzureBus(HttpRequest httpRequest)
        {
            ServiceBusClient client;
            ServiceBusSender sender;
            const int numOfMessages = 3;

            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            client = new ServiceBusClient(connectionStringServiceBus, new DefaultAzureCredential(), clientOptions);
            sender = client.CreateSender("service-bus-queues-dp");

            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            string body = await new StreamReader(httpRequest.Body).ReadToEndAsync();

            for (int i = 1; i <= numOfMessages; i++)
            {
                if (!messageBatch.TryAddMessage(new ServiceBusMessage(body)))
                {
                    throw new Exception($"The message {i} is too large to fit in the batch.");
                }
            }

            try
            {
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        private static async Task WriteToBlod(HttpRequest httpRequest)
        {
            var blobServiceClient = new BlobServiceClient(connectionStringBlob);
            var containerName = "orderitems";
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var fileName = "item_" + Guid.NewGuid().ToString() + ".json";
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(httpRequest.Body);
        }
    }
}
        
*/