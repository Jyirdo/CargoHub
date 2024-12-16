using Cargohub_V2.DataConverters;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Cargohub_V2.Models
{
    public class Order
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("source_id")]
        public int SourceId { get; set; }

        [JsonProperty("order_date")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime OrderDate { get; set; }

        [JsonProperty("request_date")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime RequestDate { get; set; }

        [JsonProperty("reference")]
        public string? Reference { get; set; }

        [JsonProperty("reference_extra")]
        public string? Reference_extra { get; set; }

        [JsonProperty("order_status")]
        public string? Order_status { get; set; }

        [JsonProperty("notes")]
        public string? Notes { get; set; }

        [JsonProperty("shipping_notes")]
        public string? ShippingNotes { get; set; }

        [JsonProperty("picking_notes")]
        public string? PickingNotes { get; set; }

        [JsonProperty("warehouse_id")]
        public int WarehouseId { get; set; }

        [JsonProperty("ship_to")]
        public string? ShipTo { get; set; }

        [JsonProperty("bill_to")]
        public string? BillTo { get; set; }

        [JsonProperty("shipment_id")]
        public int ShipmentId { get; set; }

        [JsonProperty("total_amount")]
        public double TotalAmount { get; set; }

        [JsonProperty("total_discount")]
        public double TotalDiscount { get; set; }

        [JsonProperty("total_tax")]
        public double TotalTax { get; set; }

        [JsonProperty("total_surcharge")]
        public double TotalSurcharge { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("items")]
        public List<OrderStock> Stocks { get; set; } = new List<OrderStock>();
    }
}
