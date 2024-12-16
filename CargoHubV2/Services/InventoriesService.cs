using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargohubV2.Contexts;
using CargohubV2.Models;
using Microsoft.EntityFrameworkCore;

namespace CargohubV2.Services
{
    public class InventoriesService
    {
        private readonly CargoHubDbContext _context;

        public InventoriesService(CargoHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<Inventory>> GetAllInventoriesAsync()
        {
            return await _context.Inventories
                .Include(i => i.Description)
                .Include(i => i.Locations)
                .Include(i => i.TotalOnHand)
                .Include(i => i.TotalOrdered)
                .OrderBy(i => i.Id) // Order by Id in ascending order
                .Take(10)
                .ToListAsync();
        }

        public async Task<Inventory?> GetInventoriesByIdAsync(int id)
        {
            return await _context.Inventories
                .Include(i => i.Description)
                .Include(i => i.Locations)
                .Include(i => i.TotalOnHand)
                .Include(i => i.TotalOrdered)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Inventory> AddInventoryAsync(Inventory newInventory)
        {
            // Get the latest ID
            var lastInventory = await _context.Inventories
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync();

            // Generate ID (increment from last ID)
            if (lastInventory != null)
            {
                newInventory.Id = lastInventory.Id;
            }
            else
            {
                newInventory.Id = 1;
            }

            DateTime CreatedAt = DateTime.UtcNow;
            DateTime UpdatedAt = DateTime.UtcNow;

            newInventory.CreatedAt = new DateTime(CreatedAt.Year, CreatedAt.Month, CreatedAt.Day, CreatedAt.Hour, CreatedAt.Minute, CreatedAt.Second, DateTimeKind.Utc);
            newInventory.UpdatedAt = new DateTime(UpdatedAt.Year, UpdatedAt.Month, UpdatedAt.Day, UpdatedAt.Hour, UpdatedAt.Minute, UpdatedAt.Second, DateTimeKind.Utc);

            _context.Inventories.Add(newInventory);
            await _context.SaveChangesAsync();
            return newInventory;
        }

        public async Task<bool> UpdateInventoryAsync(int id, Inventory updatedInventory)
        {
            var existingInvenvotry = await _context.Inventories.FindAsync(id);

            if (existingInvenvotry == null)
            {
                return false;
            }

            existingInvenvotry.Id = updatedInventory.Id;
            existingInvenvotry.ItemId = updatedInventory.ItemId;
            existingInvenvotry.Description = updatedInventory.Description;
            existingInvenvotry.ItemReference = updatedInventory.ItemReference;
            existingInvenvotry.Locations = updatedInventory.Locations;
            existingInvenvotry.TotalOnHand = updatedInventory.TotalOnHand;
            existingInvenvotry.TotalExpected = updatedInventory.TotalExpected;
            existingInvenvotry.TotalOrdered = updatedInventory.TotalOrdered;
            existingInvenvotry.TotalAllocated = updatedInventory.TotalAllocated;
            existingInvenvotry.TotalAvailable = updatedInventory.TotalAvailable;
            existingInvenvotry.CreatedAt = updatedInventory.CreatedAt;
            existingInvenvotry.UpdatedAt = updatedInventory.UpdatedAt;

            existingInvenvotry.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveInventoryAsync(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);

            if (inventory == null)
            {
                return false;
            }

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
