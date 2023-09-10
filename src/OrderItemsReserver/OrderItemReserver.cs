using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace OrderItemsReserver;

public class OrderItemReserver
{
    private const string connectionStringBlob = "DefaultEndpointsProtocol=https;AccountName=eshoponwebstorage2023;AccountKey=HKT/lTQIWK7kRnYiJNRSvfrzhF6OWn9L6sOYuTR7ptx/Of/8pFJoJMqksfVvJ1aaGtn7uMIZIQjb+ASt0ewMtA==;EndpointSuffix=core.windows.net";
    private const string AzureWebJobsServiceBus = "Endpoint=sb://eshoponweb-dp.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ZRBXlncaNrntMLGYByRxX8iiKGeY1qNg++ASbGwTY8g=";

    [FunctionName("OrderItemReserver")]
    public async Task Run([ServiceBusTrigger("service-bus-queues-dp"
        ,Connection = "AzureWebJobsServiceBus"
        )]
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
            await blobClient.UploadAsync(GenerateStreamFromString(myQueueItem));

            Console.WriteLine("Done");
        }
        catch (Exception ex)
        {
            log.LogError(ex.Message, ex);
        }
    }


    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}
