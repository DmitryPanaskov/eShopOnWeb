using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace OrderItemsReserver;

public class OrderItemReserver
{
    private const string connectionStringBlob = "DefaultEndpointsProtocol=https;AccountName=eshoponwebstorage2023;AccountKey=HKT/lTQIWK7kRnYiJNRSvfrzhF6OWn9L6sOYuTR7ptx/Of/8pFJoJMqksfVvJ1aaGtn7uMIZIQjb+ASt0ewMtA==;EndpointSuffix=core.windows.net";

    [FunctionName("OrderItemReserver")]
    public async Task Run([ServiceBusTrigger("service-bus-queues-dp", Connection = "Endpoint=sb://eshoponweb-dp.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ZRBXlncaNrntMLGYByRxX8iiKGeY1qNg++ASbGwTY8g=")]string myQueueItem, ILogger log)
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
