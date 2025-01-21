using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CargohubV2.Models;
using CargohubV2.Contexts;
using CargohubV2.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargohubV2.Services
{
    public class ReportingService
    {
        private readonly List<Order> _orders;
        private readonly CargoHubDbContext _context;


        // Sample data for the sake of this example (you can replace it with your actual database context)
        public ReportingService(CargoHubDbContext context)
        {
            _context = context; // This will hold the orders
            // Initialize _orders with sample data (or fetch from the DB context)
        }

        // Get Report Data
        // Get Report Data
        public ReportResult GetWarehouseReport(int warehouseId)
        {
            // Filter orders by warehouseId
            var ordersInWarehouse = _context.Orders
                .Where(o => o.WarehouseId == warehouseId)
                .Include(o => o.Stocks)  // Eager load Stocks to prevent lazy loading issues
                .ToList();

            // Log the orders fetched for the warehouseId
            Console.WriteLine($"Total orders fetched: {ordersInWarehouse.Count}");

            // Initialize totals for the report
            int totalOrders = ordersInWarehouse.Count;
            int totalItems = 0;

            // Sum up quantities for each order related to the warehouse
            foreach (var order in ordersInWarehouse)
            {
                // For each order, check if Stocks is null or empty
                if (order.Stocks != null && order.Stocks.Any())
                {
                    var orderTotalQuantity = order.Stocks
                        .Where(stock => stock.OrderId == order.Id)  // Ensure stock is linked to the correct order
                        .Sum(stock => stock.Quantity);  // Sum the quantities for that order

                    totalItems += orderTotalQuantity;
                    Console.WriteLine($"  Total quantity for Order {order.Id}: {orderTotalQuantity}");
                }
                else
                {
                    Console.WriteLine($"  No stocks found for Order {order.Id}");
                }
            }

            return new ReportResult
            {
                TotalOrders = totalOrders,
                TotalItems = totalItems
            };
        }
        // Generate CSV for Report
        public string GenerateCsvReport(int warehouseId)
        {
            // Fetch the report data for the given warehouse ID
            var ordersInWarehouse = _context.Orders
                .Where(o => o.WarehouseId == warehouseId)
                .ToList();

            // Start building the CSV content
            var stringBuilder = new StringBuilder();

            // Loop through the orders and format the output
            foreach (var order in ordersInWarehouse)
            {
                stringBuilder.AppendLine($"Order ID: {order.Id}, Warehouse ID: {order.WarehouseId}, Order price: {order.TotalAmount}");
            }

            return stringBuilder.ToString();
        }

        // Generate CSV for Locations
        public string GenerateCsvForLocations(int warehouseId)
        {
            // Fetch locations for the given warehouse ID
            var locations = _context.Locations
                .Where(loc => loc.WarehouseId == warehouseId)
                .Select(loc => new { loc.Code, loc.Name })
                .ToList();

            // Start building the CSV content
            var stringBuilder = new StringBuilder();

            // Append CSV header
            stringBuilder.AppendLine("Warehouse ID, Location Code, Location Name");

            // Append each location's details
            foreach (var loc in locations)
            {
                stringBuilder.AppendLine($"{warehouseId}, {loc.Code}, {loc.Name}");
            }

            return stringBuilder.ToString();
        }

        // Add this method to the ReportingService class
        public LocationResult GetLocationsByWarehouseId(int warehouseId)
        {
            // Fetch locations where WarehouseId matches the input
            var locations = _context.Locations
                .Where(loc => loc.WarehouseId == warehouseId)
                .Select(loc => new { loc.Code, loc.Name }) // Adjust fields as per your `Locations` table
                .ToList();

            // Log fetched locations
            Console.WriteLine($"Total locations fetched for Warehouse ID {warehouseId}: {locations.Count}");

            // Convert to a LocationResult DTO
            return new LocationResult
            {
                AllLocationOfWarehouseID = locations
                    .Select(loc => $"Code: {loc.Code}, Name: {loc.Name}")
                    .ToList()
            };
        }
        public revenewResultOrders GetRevenueBetweenDates(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.ToUniversalTime();  // Ensure startDate is in UTC
            endDate = endDate.ToUniversalTime();
            // Fetch orders within the specified date range
            var ordersInRange = _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .ToList();

            // Sum up the total amounts of these orders
            var totalRevenue = ordersInRange.Sum(o => o.TotalAmount);
            var totalRevenuerounded = Math.Round(totalRevenue, 2);

            // Log the result for debugging purposes
            Console.WriteLine($"Total revenue between {startDate} and {endDate}: {totalRevenuerounded}");

            // Return the result in the DTO
            return new revenewResultOrders
            {
                TotalRevenewInBetweenDates = totalRevenuerounded
            };
        }

    }

    // DTO to hold the report result
    public class ReportResult
    {
        public int TotalOrders { get; set; }
        public int TotalItems { get; set; }
    }
    public class LocationResult
    {
        public List<string>? AllLocationOfWarehouseID { get; set; }
    }

    public class revenewResultOrders
    {
        public double TotalRevenewInBetweenDates { get; set; }
    }

}
