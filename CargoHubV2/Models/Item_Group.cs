using Cargohub_V2.DataConverters;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace Cargohub_V2.Models
{
    public class Item_Group
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Numbers and special characters are not allowed")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime UpdatedAt { get; set; }


    }
}
