using Cargohub_V2.DataConverters;
using Newtonsoft.Json;

namespace Cargohub_V2.Models
{
    public class Shipment
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("order_id")]
        public int OrderId { get; set; }

        [JsonProperty("source_id")]
        public int SourceId { get; set; }

        [JsonProperty("order_date")]
        public string? OrderDate { get; set; }

        [JsonProperty("request_date")]
        public string? RequestDate { get; set; }

        [JsonProperty("shipment_date")]
        public string? ShipmentDate { get; set; }

        [JsonProperty("shipment_type")]
        public string? ShipmentType { get; set; }

        [JsonProperty("shipment_status")]
        public string? ShipmentStatus { get; set; }

        [JsonProperty("notes")]
        public string? Notes { get; set; }

        [JsonProperty("carrier_code")]
        public string? CarrierCode { get; set; }

        [JsonProperty("carrier_description")]
        public string? CarrierDescription { get; set; }

        [JsonProperty("service_code")]
        public string? ServiceCode { get; set; }

        [JsonProperty("payment_type")]
        public string? PaymentType { get; set; }

        [JsonProperty("transfer_mode")]
        public string? TransferMode { get; set; }

        [JsonProperty("total_package_count")]
        public int TotalPackageCount { get; set; }

        [JsonProperty("total_package_weight")]
        public double TotalPackageWeight { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("items")]
        public List<ShipmentStock> Stocks { get; set; } = new List<ShipmentStock>();
    }
}
