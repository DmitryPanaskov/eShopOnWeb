using System.Collections.Generic;
using Newtonsoft.Json;

namespace OrderItemsReserver.Entities
{
    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey { get; set; }

        public Address Address { get; set; }

        public List<ListOfItems> ListOfItems { get; set; }

        public string FinalPrice { get; set; }
    }
}
