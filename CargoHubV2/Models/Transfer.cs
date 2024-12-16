using CargohubV2.DataConverters;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;

namespace CargohubV2.Models
{
    public class Transfer
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("reference")]
        public string? Reference { get; set; }

        [JsonProperty("transfer_from")]
        public int TransferFrom { get; set; }

        [JsonProperty("transfer_to")]
        public int TransferTo { get; set; }

        [JsonProperty("transfer_status")]
        public string? TransferStatus { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("items")]
        public List<TransferStock>? Stocks { get; set; }
    }
}
