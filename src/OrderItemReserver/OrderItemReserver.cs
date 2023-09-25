using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace OrderItemReserver
{
    public class OrderItemReserver
    {
        private const string connectionStringBlob = "";

        [FunctionName("OrderItemReserver")]
        public async Task Run([ServiceBusTrigger("service-bus-queus-dp", Connection = "AzureWebJobsService")]string myQueueItem, ILogger log)
        {
            Console.WriteLine("Strat");
            log.LogInformation("Strat DeliveryOrderProcessor");

            var blobServiceClient = new BlobServiceClient(connectionStringBlob);
            var containerName = "orderitems";
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var fileName = "item_" + Guid.NewGuid().ToString() + ".json";
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(GenerateStringFromString(myQueueItem));

            log.LogInformation("Done DeliveryOrderProcessor");
            Console.WriteLine("Done");
        }

        private Stream GenerateStringFromString(string myQueueItem)
        {
            var stream = new MemoryStream();
            var write = new BinaryWriter(stream);
            write.Write(myQueueItem);   
            write.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
