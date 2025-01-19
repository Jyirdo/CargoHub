using System.Collections.Generic;
using System.Threading.Tasks;
using CargohubV2.Contexts;
using CargohubV2.Models;
using Microsoft.EntityFrameworkCore;

namespace CargohubV2.Services
{
    public class ItemTypeService
    {
        private readonly CargoHubDbContext _context;

        public ItemTypeService(CargoHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<Item_Type>> GetAllItemTypesAsync(int amount)
        {
            return await _context.Items_Types.Take(amount).ToListAsync();
        }

        public async Task<Item_Type> GetItemTypeByIdAsync(int id)
        {
            return await _context.Items_Types.FirstOrDefaultAsync(it => it.Id == id);
        }

        public async Task<Item_Type> GetItemTypeByNameAsync(string name)
        {
            return await _context.Items_Types.FirstOrDefaultAsync(it => it.Name == name);
        }

        public async Task<Item_Type> AddItemTypeAsync(Item_Type newItemType)
        {
            DateTime CreatedAt = DateTime.UtcNow;
            DateTime UpdatedAt = DateTime.UtcNow;

            newItemType.CreatedAt = new DateTime(CreatedAt.Year, CreatedAt.Month, CreatedAt.Day, CreatedAt.Hour, CreatedAt.Minute, CreatedAt.Second, DateTimeKind.Utc);
            newItemType.UpdatedAt = new DateTime(UpdatedAt.Year, UpdatedAt.Month, UpdatedAt.Day, UpdatedAt.Hour, UpdatedAt.Minute, UpdatedAt.Second, DateTimeKind.Utc);

            _context.Items_Types.Add(newItemType);
            await _context.SaveChangesAsync();
            return newItemType;
        }

        public async Task<Item_Type> UpdateItemTypeAsync(int id, Item_Type updatedItemType)
        {
            var existingItemType = await _context.Items_Types.FindAsync(id);

            if (existingItemType == null)
            {
                return null;
            }

            existingItemType.Name = updatedItemType.Name;
            existingItemType.Description = updatedItemType.Description;

            DateTime UpdatedAt = DateTime.UtcNow;
            existingItemType.UpdatedAt = new DateTime(UpdatedAt.Year, UpdatedAt.Month, UpdatedAt.Day, UpdatedAt.Hour, UpdatedAt.Minute, UpdatedAt.Second, DateTimeKind.Utc);

            _context.Items_Types.Update(existingItemType);
            await _context.SaveChangesAsync();
            return updatedItemType;
        }

        public async Task<bool> DeleteItemTypeAsync(int id)
        {
            var itemType = await _context.Items_Types.FindAsync(id);

            if (itemType == null)
            {
                return false;
            }

            itemType.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
