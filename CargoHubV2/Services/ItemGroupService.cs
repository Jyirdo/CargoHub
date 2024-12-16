using System.Collections.Generic;
using System.Threading.Tasks;
using Cargohub_V2.Contexts;
using Cargohub_V2.Models;
using Microsoft.EntityFrameworkCore;

namespace Cargohub_V2.Services
{
    public class ItemGroupService
    {
        private readonly CargoHubDbContext _context;

        public ItemGroupService(CargoHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<Item_Group>> GetAllItemGroupsAsync()
        {
            return await _context.Items_Groups.Take(100).ToListAsync();
        }


        public async Task<Item_Group> GetItemGroupByIdAsync(int id)
        {
            return await _context.Items_Groups.FirstOrDefaultAsync(ig => ig.Id == id);
        }

        public async Task<Item_Group> GetItemGroupByNameAsync(string name)
        {
            return await _context.Items_Groups.FirstOrDefaultAsync(ig => ig.Name == name);
        }

        public async Task<Item_Group> AddItemGroupAsync(Item_Group itemGroup)
        {
            DateTime CreatedAt = DateTime.UtcNow;
            DateTime UpdatedAt = DateTime.UtcNow;

            itemGroup.CreatedAt = new DateTime(CreatedAt.Year, CreatedAt.Month, CreatedAt.Day, CreatedAt.Hour, CreatedAt.Minute, CreatedAt.Second, DateTimeKind.Utc);
            itemGroup.UpdatedAt = new DateTime(UpdatedAt.Year, UpdatedAt.Month, UpdatedAt.Day, UpdatedAt.Hour, UpdatedAt.Minute, UpdatedAt.Second, DateTimeKind.Utc);

            _context.Items_Groups.Add(itemGroup);
            await _context.SaveChangesAsync();
            return itemGroup;
        }

        public async Task<Item_Group> UpdateItemGroupAsync(int id, Item_Group updatedItemGroup)
        {
            var existingItemGroup = await _context.Items_Groups.FindAsync(id);

            if (existingItemGroup == null)
            {
                return null; // Item group not found
            }

            // Update fields from the payload
            existingItemGroup.Name = updatedItemGroup.Name;
            existingItemGroup.Description = updatedItemGroup.Description;

            DateTime UpdatedAt = DateTime.UtcNow;
            existingItemGroup.UpdatedAt = new DateTime(UpdatedAt.Year, UpdatedAt.Month, UpdatedAt.Day, UpdatedAt.Hour, UpdatedAt.Minute, UpdatedAt.Second, DateTimeKind.Utc);

            _context.Items_Groups.Update(existingItemGroup);
            await _context.SaveChangesAsync();
            return existingItemGroup;
        }


        public async Task<bool> RemoveItemGroupAsync(int id)
        {
            var itemGroup = await _context.Items_Groups.FindAsync(id);

            if (itemGroup == null)
                return false;

            _context.Items_Groups.Remove(itemGroup);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
