using System.Collections.Generic;
using System.Threading.Tasks;
using Cargohub_V2.Contexts;
using Cargohub_V2.Models;
using Microsoft.EntityFrameworkCore;

namespace Cargohub_V2.Services
{
    public class ItemLineService
    {
        private readonly CargoHubDbContext _context;

        public ItemLineService(CargoHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<Item_Line>> GetAllItemLinesAsync()
        {
            return await _context.Items_Lines.Take(100).ToListAsync();
        }

        public async Task<Item_Line> GetItemLineByIdAsync(int id)
        {
            return await _context.Items_Lines.FirstOrDefaultAsync(il => il.Id == id);
        }

        public async Task<Item_Line> GetItemLineByNameAsync(string name)
        {
            return await _context.Items_Lines.FirstOrDefaultAsync(il => il.Name == name);
        }

        public async Task<Item_Line> AddItemLineAsync(Item_Line newItemLine)
        {

            DateTime CreatedAt = DateTime.UtcNow;
            DateTime UpdatedAt = DateTime.UtcNow;

            newItemLine.CreatedAt = new DateTime(CreatedAt.Year, CreatedAt.Month, CreatedAt.Day, CreatedAt.Hour, CreatedAt.Minute, CreatedAt.Second, DateTimeKind.Utc);
            newItemLine.UpdatedAt = new DateTime(UpdatedAt.Year, UpdatedAt.Month, UpdatedAt.Day, UpdatedAt.Hour, UpdatedAt.Minute, UpdatedAt.Second, DateTimeKind.Utc);

            _context.Items_Lines.Add(newItemLine);
            await _context.SaveChangesAsync();
            return newItemLine;
        }

        public async Task<Item_Line> UpdateItemLineAsync(int id, Item_Line updatedItemLine)
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

        public async Task<bool> DeleteItemLineAsync(int id)
        {
            var itemLine = await _context.Items_Lines.FindAsync(id);

            if (itemLine == null)
            {
                return false;
            }

            _context.Items_Lines.Remove(itemLine);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
