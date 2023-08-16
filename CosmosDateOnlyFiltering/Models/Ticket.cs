using Newtonsoft.Json;

namespace CosmosDateOnlyFiltering.Models
{
    public class Ticket
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("createdAt")]
        public DateOnly CreatedAt { get; set; }
    }
}
