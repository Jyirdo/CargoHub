using CargohubV2.Contexts;
using CargohubV2.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargohubV2.Services
{
    public class OrderService
    {
        private readonly CargoHubDbContext _context;

        public OrderService(CargoHubDbContext context)
        {
            _context = context;
        }

        public virtual async Task<List<Order>> GetAllAsync() // Virtual toegevoegd
        {
            return await _context.Orders.ToListAsync();
        }

        public virtual async Task<List<Order>> GetAllOrdersAsync(int amount) // Virtual toegevoegd
        {
            return await _context.Orders
                .OrderBy(o => o.Id)
                .Take(amount)
                .ToListAsync();
        }

        public virtual async Task<Order?> GetOrderByIdAsync(int orderId) // Virtual toegevoegd
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public virtual async Task<List<OrderStock>> GetItemsInOrderAsync(int orderId) // Virtual toegevoegd
        {
            var order = await GetOrderByIdAsync(orderId);
            return order?.Stocks ?? new List<OrderStock>();
        }

        public virtual async Task<List<Order>> GetOrdersForShipmentAsync(int shipmentId) // Virtual toegevoegd
        {
            return await _context.Orders
                .Where(o => o.ShipmentId == shipmentId)
                .ToListAsync();
        }

        public virtual async Task<List<Order>> GetOrdersForClientAsync(string clientId) // Virtual toegevoegd
        {
            return await _context.Orders
                .Where(o => o.ShipTo == clientId || o.BillTo == clientId)
                .ToListAsync();
        }

        public virtual async Task<Order> AddOrderAsync(Order newOrder) // Virtual toegevoegd
        {
            newOrder.CreatedAt = DateTime.UtcNow;
            newOrder.UpdatedAt = DateTime.UtcNow;

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();
            return newOrder;
        }

        public virtual async Task<bool> UpdateOrderAsync(int orderId, Order updatedOrder) // Virtual toegevoegd
        {
            var existingOrder = await _context.Orders
                .Include(o => o.Stocks)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (existingOrder == null)
            {
                return false;
            }

            // Velden bijwerken
            existingOrder.SourceId = updatedOrder.SourceId;
            existingOrder.OrderDate = updatedOrder.OrderDate;
            existingOrder.RequestDate = updatedOrder.RequestDate;
            existingOrder.Reference = updatedOrder.Reference;
            existingOrder.Reference_extra = updatedOrder.Reference_extra;
            existingOrder.Order_status = updatedOrder.Order_status;
            existingOrder.Notes = updatedOrder.Notes;
            existingOrder.ShippingNotes = updatedOrder.ShippingNotes;
            existingOrder.PickingNotes = updatedOrder.PickingNotes;
            existingOrder.WarehouseId = updatedOrder.WarehouseId;
            existingOrder.ShipTo = updatedOrder.ShipTo;
            existingOrder.BillTo = updatedOrder.BillTo;
            existingOrder.ShipmentId = updatedOrder.ShipmentId;
            existingOrder.TotalAmount = updatedOrder.TotalAmount;
            existingOrder.TotalDiscount = updatedOrder.TotalDiscount;
            existingOrder.TotalTax = updatedOrder.TotalTax;
            existingOrder.TotalSurcharge = updatedOrder.TotalSurcharge;
            existingOrder.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> DeleteOrderAsync(int orderId) // Virtual toegevoegd
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return false;
            }

            order.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
