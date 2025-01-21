using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargohubV2.Contexts;
using CargohubV2.Models;
using Microsoft.EntityFrameworkCore;

namespace CargohubV2.Services
{
    public class ItemService
    {
        private readonly CargoHubDbContext _context;

        public ItemService(CargoHubDbContext context)
        {
            _context = context;
        }

        public virtual async Task<List<Item>> GetAllItemsAsync(int amount)
        {
            return await _context.Items
                .Include(i => i.ItemLine)
                .Include(i => i.ItemGroup)
                .Include(i => i.ItemType)
                .Include(i => i.Supplier)
                .OrderBy(i => i.Id) // Order by Id in ascending order
                .Take(amount)
                .ToListAsync();
        }

        public virtual async Task<Item?> GetItemByUidAsync(string uid)
        {
            return await _context.Items
                .Include(i => i.ItemLine)
                .Include(i => i.ItemGroup)
                .Include(i => i.ItemType)
                .Include(i => i.Supplier)
                .FirstOrDefaultAsync(i => i.UId == uid);
        }

        public virtual async Task<List<Item>> GetItemsByItemLineAsync(int itemLineId)
        {
            return await _context.Items
                .Where(i => i.ItemLineId == itemLineId)
                .Include(i => i.ItemLine)
                .ToListAsync();
        }

        public virtual async Task<List<Item>> GetItemsByItemGroupAsync(int itemGroupId)
        {
            return await _context.Items
                .Where(i => i.ItemGroupId == itemGroupId)
                .Include(i => i.ItemGroup)
                .ToListAsync();
        }

        public virtual async Task<List<Item>> GetItemsByItemTypeAsync(int itemTypeId)
        {
            return await _context.Items
                .Where(i => i.ItemTypeId == itemTypeId)
                .Include(i => i.ItemType)
                .ToListAsync();
        }

        public virtual async Task<List<Item>> GetItemsBySupplierAsync(int supplierId)
        {
            return await _context.Items
                .Where(i => i.SupplierId == supplierId)
                .Include(i => i.Supplier)
                .ToListAsync();
        }

        public virtual async Task<bool> UpdateItemAsync(int id, Item updatedItem)
        {
            var existingItem = await _context.Items.FindAsync(id);

            if (existingItem == null)
            {
                return false;
            }

            existingItem.UId = updatedItem.UId;
            existingItem.Code = updatedItem.Code;
            existingItem.Description = updatedItem.Description;
            existingItem.ShortDescription = updatedItem.ShortDescription;
            existingItem.UpcCode = updatedItem.UpcCode;
            existingItem.ModelNumber = updatedItem.ModelNumber;
            existingItem.CommodityCode = updatedItem.CommodityCode;
            existingItem.ItemLineId = updatedItem.ItemLineId;
            existingItem.ItemGroupId = updatedItem.ItemGroupId;
            existingItem.ItemTypeId = updatedItem.ItemTypeId;
            existingItem.UnitPurchaseQuantity = updatedItem.UnitPurchaseQuantity;
            existingItem.UnitOrderQuantity = updatedItem.UnitOrderQuantity;
            existingItem.PackOrderQuantity = updatedItem.PackOrderQuantity;
            existingItem.SupplierId = updatedItem.SupplierId;
            existingItem.SupplierCode = updatedItem.SupplierCode;
            existingItem.SupplierPartNumber = updatedItem.SupplierPartNumber;

            existingItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> RemoveItemAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return false;
            }

            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task PopulateWeightInKgAsync() 
        {
            var random = new Random();
            var items = await _context.Items.ToListAsync();

            foreach (var item in items)
            {
                item.WeightInKg = random.Next(1, 101);
            }

            await _context.SaveChangesAsync();
        }
    }
}
