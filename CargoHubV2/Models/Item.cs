using CargohubV2.DataConverters;
using Newtonsoft.Json;

namespace CargohubV2.Models
{
    public class Item
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("uid")]
        public string? UId { get; set; }

        [JsonProperty("code")]
        public string? Code { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("short_description")]
        public string? ShortDescription { get; set; }

        [JsonProperty("upc_code")]
        public string? UpcCode { get; set; }

        [JsonProperty("model_number")]
        public string? ModelNumber { get; set; }

        [JsonProperty("commodity_code")]
        public string? CommodityCode { get; set; }
        public Item_Line? ItemLine { get; set; }

        [JsonProperty("item_line")]
        public int? ItemLineId { get; set; }
        public Item_Group? ItemGroup { get; set; }

        [JsonProperty("item_group")]
        public int? ItemGroupId { get; set; }
        public Item_Type? ItemType { get; set; }

        [JsonProperty("item_type")]
        public int? ItemTypeId { get; set; }

        [JsonProperty("unit_purchase_quantity")]
        public int UnitPurchaseQuantity { get; set; }

        [JsonProperty("unit_order_quantity")]
        public int UnitOrderQuantity { get; set; }

        [JsonProperty("pack_order_quantity")]
        public int PackOrderQuantity { get; set; }

        [JsonProperty("supplier")]
        public Supplier? Supplier { get; set; }

        [JsonProperty("supplier_id")]
        public int SupplierId { get; set; }

        [JsonProperty("supplier_code")]
        public string? SupplierCode { get; set; }

        [JsonProperty("supplier_part_number")]
        public string? SupplierPartNumber { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
