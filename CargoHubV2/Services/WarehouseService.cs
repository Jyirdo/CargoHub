using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargohubV2.Contexts;
using CargohubV2.Models;
using Microsoft.EntityFrameworkCore;

namespace CargohubV2.Services
{
    public class WarehouseService
    {
        private readonly CargoHubDbContext _context;

        public WarehouseService(CargoHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<Warehouse>> GetAllWarehousesAsync(int amount)
        {
            return await _context.Warehouses.Take(amount).ToListAsync();
        }

        public async Task<Warehouse> GetWarehouseByIdAsync(int id)
        {
            return await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<List<Warehouse>> GetWarehousesByCityAsync(string city)
        {
            return await _context.Warehouses.Where(w => w.City == city).ToListAsync();
        }

        public async Task<Warehouse> AddWarehouseAsync(Warehouse warehouse)
        {
            warehouse.CreatedAt = DateTime.UtcNow;
            warehouse.UpdatedAt = DateTime.UtcNow;

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            return warehouse;
        }

        public async Task<Warehouse> UpdateWarehouseAsync(int id, Warehouse warehouse)
        {
            var existingWarehouse = await _context.Warehouses.FindAsync(id);
            if (existingWarehouse == null) return null;

            existingWarehouse.Name = warehouse.Name;
            existingWarehouse.Address = warehouse.Address;
            existingWarehouse.City = warehouse.City;
            existingWarehouse.Country = warehouse.Country;
            existingWarehouse.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return existingWarehouse;
        }

        public async Task<bool> DeleteWarehouseByIdAsync(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null) return false;

            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
