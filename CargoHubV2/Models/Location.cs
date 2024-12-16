using Cargohub_V2.DataConverters;
using Newtonsoft.Json;

namespace Cargohub_V2.Models
{
    public class Location
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("warehouse_id")]
        public int WarehouseId { get; set; }

        [JsonProperty("code")]
        public string? Code { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime UpdatedAt { get; set; }
    }
}
