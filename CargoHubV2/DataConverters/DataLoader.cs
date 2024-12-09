namespace Cargohub_V2.DataConverters
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using Cargohub_V2.Contexts;
    using Cargohub_V2.Models;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;


    public class MultiFormatDateConverter : IsoDateTimeConverter
    {
        public MultiFormatDateConverter()
        {
            DateTimeStyles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal;
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(DateTime) || objectType == typeof(DateTime?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            var dateStr = reader.Value.ToString();
            if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.AssumeUniversal, out var dt) ||
                DateTime.TryParseExact(dateStr, "yyyy-MM-ddTHH:mm:ssZ", null, DateTimeStyles.AssumeUniversal, out dt))
            {
                return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            }

            throw new JsonSerializationException($"Unable to parse '{dateStr}' as a date.");
        }
    }

    public class DataLoader
    {

        public static DateTime ToUtc(DateTime dateTime)
        {
            return dateTime.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
                : dateTime.ToUniversalTime();
        }
        public static List<T> LoadDataFromFile<T>(string filePath)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new MultiFormatDateConverter() },
                NullValueHandling = NullValueHandling.Ignore // Skip null values
            };

            using (StreamReader reader = new StreamReader(filePath))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<List<T>>(json, settings);
            }
        }
        public static void ImportData(CargoHubDbContext context)
        {
            // Import Clients
            var clients = LoadDataFromFile<Client>("data/clients.json");
            foreach (var client in clients)
            {
                client.CreatedAt = ToUtc(client.CreatedAt);
                client.UpdatedAt = ToUtc(client.UpdatedAt);
                client.Id = 0; // Resetting the Id to 0
            }
            context.Clients.AddRange(clients);
            context.SaveChanges();

            // Import Inventories
            var inventories = LoadDataFromFile<Inventory>("data/inventories.json");
            foreach (var inventory in inventories)
            {
                inventory.CreatedAt = ToUtc(inventory.CreatedAt);
                inventory.UpdatedAt = ToUtc(inventory.UpdatedAt);
                inventory.Id = 0; // Resetting the Id to 0
            }
            context.Inventories.AddRange(inventories);
            context.SaveChanges();

            // Import Suppliers
            var suppliers = LoadDataFromFile<Supplier>("data/suppliers.json");
            foreach (var supplier in suppliers)
            {
                supplier.CreatedAt = ToUtc(supplier.CreatedAt);
                supplier.UpdatedAt = ToUtc(supplier.UpdatedAt);
                supplier.Id = 0; // Resetting the Id to 0
            }
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            // Import Item Groups before Items
            var itemGroups = LoadDataFromFile<Item_Group>("data/item_groups.json");
            foreach (var itemGroup in itemGroups)
            {
                itemGroup.CreatedAt = ToUtc(itemGroup.CreatedAt);
                itemGroup.UpdatedAt = ToUtc(itemGroup.UpdatedAt);
                itemGroup.Id = 0; // Resetting the Id to 0
            }
            context.Items_Groups.AddRange(itemGroups);
            context.SaveChanges(); // Ensure Item Groups are saved first

            // Import Item Lines before Items
            var itemLines = LoadDataFromFile<Item_Line>("data/item_lines.json");
            foreach (var itemLine in itemLines)
            {
                itemLine.CreatedAt = ToUtc(itemLine.CreatedAt);
                itemLine.UpdatedAt = ToUtc(itemLine.UpdatedAt);

                itemLine.Id = 0; // Resetting the Id to 0
            }
            context.Items_Lines.AddRange(itemLines);
            context.SaveChanges(); // Ensure Item Lines are saved first

            // Import Item Types before Items
            var itemTypes = LoadDataFromFile<Item_Type>("data/item_types.json");
            foreach (var itemType in itemTypes)
            {
                itemType.CreatedAt = ToUtc(itemType.CreatedAt);
                itemType.UpdatedAt = ToUtc(itemType.UpdatedAt);
                itemType.Id = 0; // Resetting the Id to 0
            }
            context.Items_Types.AddRange(itemTypes);
            context.SaveChanges(); // Ensure Item Types are saved first

            // Now Import Items
            var items = LoadDataFromFile<Item>("data/items.json");
            foreach (var item in items)
            {
                item.CreatedAt = ToUtc(item.CreatedAt);
                item.UpdatedAt = ToUtc(item.UpdatedAt);
                item.Id = 0; // Resetting the Id to 0
                             // Optionally, ensure the ItemLineId and ItemTypeId are valid before adding
            }
            context.Items.AddRange(items);
            context.SaveChanges();

            // Import Warehouses
            var warehouses = LoadDataFromFile<Warehouse>("data/warehouses.json");
            foreach (var warehouse in warehouses)
            {
                warehouse.CreatedAt = ToUtc(warehouse.CreatedAt);
                warehouse.UpdatedAt = ToUtc(warehouse.UpdatedAt);
                warehouse.Id = 0; // Resetting the Id to 0
            }
            context.Warehouses.AddRange(warehouses);
            context.SaveChanges();

            // Import Orders
            var orders = LoadDataFromFile<Order>("data/orders.json");
            foreach (var order in orders)
            {
                order.CreatedAt = ToUtc(order.CreatedAt);
                order.UpdatedAt = ToUtc(order.UpdatedAt);
                order.RequestDate = ToUtc(order.RequestDate);
                order.OrderDate = ToUtc(order.OrderDate);
                order.Id = 0; // Resetting the Id to 0
            }
            context.Orders.AddRange(orders);
            context.SaveChanges();

            // Load Shipments
            var shipments = LoadDataFromFile<Shipment>("data/shipments.json");
            foreach (var shipment in shipments)
            {
                shipment.CreatedAt = ToUtc(shipment.CreatedAt);
                shipment.UpdatedAt = ToUtc(shipment.UpdatedAt);
                shipment.Id = 0; // Resetting the Id to 0
            }
            context.Shipments.AddRange(shipments);
            context.SaveChanges();

            // Load Transfers
            var transfers = LoadDataFromFile<Transfer>("data/transfers.json");
            foreach (var transfer in transfers)
            {
                transfer.CreatedAt = ToUtc(transfer.CreatedAt);
                transfer.UpdatedAt = ToUtc(transfer.UpdatedAt);
                transfer.Id = 0; // Resetting the Id to 0
            }
            context.Transfers.AddRange(transfers);
            context.SaveChanges();

            var locations = LoadDataFromFile<Location>("data/locations.json");
            foreach (var location in locations)
            {
                location.CreatedAt = ToUtc(location.CreatedAt);
                location.UpdatedAt = ToUtc(location.UpdatedAt);
                location.Id = 0; // Resetting the Id to 0
            }
            context.Locations.AddRange(locations);
            context.SaveChanges();

            // Load Stocks from Shipments
            foreach (var shipment in shipments)
            {
                if (shipment.Stocks != null)
                {
                    foreach (var item in shipment.Stocks)
                    {
                        var stock = new ShipmentStock
                        {
                            ItemId = item.ItemId,
                            Quantity = item.Quantity,
                            ShipmentId = shipment.Id,
                        };
                        context.Stocks.Add(stock);
                    }
                }
            }
            context.SaveChanges();

            // Load Stocks from Transfers
            foreach (var transfer in transfers)
            {
                if (transfer.Stocks != null)
                {
                    foreach (var item in transfer.Stocks)
                    {
                        var stock = new TransferStock
                        {
                            ItemId = item.ItemId,
                            Quantity = item.Quantity,
                            TransferId = transfer.Id,
                        };
                        context.Stocks.Add(stock);
                    }
                    
                }
            }
            context.SaveChanges();

            // Load Stocks from Orders
            foreach (var order in orders)
            {
                if (order.Stocks != null)
                {
                    foreach (var item in order.Stocks)
                    {
                        var stock = new OrderStock
                        {
                            ItemId = item.ItemId,
                            Quantity = item.Quantity,
                            OrderId = order.Id,
                        };
                        context.Stocks.Add(stock);
                    }
                    
                }
            }
            context.SaveChanges();
        }
    }
}
