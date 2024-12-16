using CargohubV2.DataConverters;
using Newtonsoft.Json;

namespace CargohubV2.Models
{
    public class Inventory
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("item_id")]
        public string? ItemId { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("item_reference")]
        public string? ItemReference { get; set; }

        [JsonProperty("locations")]
        public List<int>? Locations { get; set; }

        [JsonProperty("total_on_hand")]
        public int TotalOnHand { get; set; }

        [JsonProperty("total_expected")]
        public int TotalExpected { get; set; }

        [JsonProperty("total_ordered")]
        public int TotalOrdered { get; set; }

        [JsonProperty("total_allocated")]
        public int TotalAllocated { get; set; }

        [JsonProperty("total_available")]
        public int TotalAvailable { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime UpdatedAt { get; set; }
    }
}
