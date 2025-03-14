using System.Collections.Generic;
using System.Threading.Tasks;
using CargohubV2.Contexts;
using CargohubV2.Models;
using Microsoft.EntityFrameworkCore;

namespace CargohubV2.Services
{
    public class ItemLineService
    {
        private readonly CargoHubDbContext _context;

        public ItemLineService(CargoHubDbContext context)
        {
            _context = context;
        }

        public virtual async Task<List<Item_Line>> GetAllItemLinesAsync(int amount)
        {
            return await _context.Items_Lines.Take(amount).ToListAsync();
        }

        public virtual async Task<Item_Line?> GetItemLineByIdAsync(int id)
        {
            return await _context.Items_Lines.FirstOrDefaultAsync(il => il.Id == id);
        }

        public virtual async Task<Item_Line?> GetItemLineByNameAsync(string name)
        {
            return await _context.Items_Lines.FirstOrDefaultAsync(il => il.Name == name);
        }

        public virtual async Task<Item_Line> AddItemLineAsync(Item_Line newItemLine)
        {

            DateTime CreatedAt = DateTime.UtcNow;
            DateTime UpdatedAt = DateTime.UtcNow;

            newItemLine.CreatedAt = new DateTime(CreatedAt.Year, CreatedAt.Month, CreatedAt.Day, CreatedAt.Hour, CreatedAt.Minute, CreatedAt.Second, DateTimeKind.Utc);
            newItemLine.UpdatedAt = new DateTime(UpdatedAt.Year, UpdatedAt.Month, UpdatedAt.Day, UpdatedAt.Hour, UpdatedAt.Minute, UpdatedAt.Second, DateTimeKind.Utc);

            _context.Items_Lines.Add(newItemLine);
            await _context.SaveChangesAsync();
            return newItemLine;
        }

        public virtual async Task<Item_Line?> UpdateItemLineAsync(int id, Item_Line updatedItemLine)
        {
            var existingItemLine = await _context.Items_Lines.FindAsync(id);

            if (existingItemLine == null)
            {
                return null;
            }

            existingItemLine.Name = updatedItemLine.Name;
            existingItemLine.Description = updatedItemLine.Description;

            DateTime UpdatedAt = DateTime.UtcNow;
            existingItemLine.UpdatedAt = new DateTime(UpdatedAt.Year, UpdatedAt.Month, UpdatedAt.Day, UpdatedAt.Hour, UpdatedAt.Minute, UpdatedAt.Second, DateTimeKind.Utc);

            _context.Items_Lines.Update(existingItemLine);
            await _context.SaveChangesAsync();
            return updatedItemLine;
        }

        public virtual async Task<bool> DeleteItemLineAsync(int id)
        {
            var itemLine = await _context.Items_Lines.FindAsync(id);

            if (itemLine == null)
            {
                return false;
            }

            itemLine.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
