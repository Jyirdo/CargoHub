using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargohubV2.Contexts;
using CargohubV2.Models;
using Microsoft.EntityFrameworkCore;

namespace CargohubV2.Services
{
    public class SupplierService
    {
        private readonly CargoHubDbContext _context;

        public SupplierService(CargoHubDbContext context)
        {
            _context = context;
        }

        // Ophalen van alle leveranciers (max 100)
        public virtual async Task<List<Supplier>> GetAllSuppliersAsync(int amount)
        {
            return await _context.Suppliers
                .OrderBy(s => s.Id)
                .Take(amount)
                .ToListAsync();
        }

        // Ophalen van een leverancier via ID
        public virtual async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
        }

        // Zoeken op naam (like filter)
        public virtual async Task<List<Supplier>> SearchSuppliersByNameAsync(string name)
        {
            return await _context.Suppliers
                .Where(s => EF.Functions.ILike(s.Name, $"%{name}%")) // Like query, case-insensitive
                .ToListAsync();
        }

        // Zoeken op stad (like filter)
        public virtual async Task<List<Supplier>> SearchSuppliersByCityAsync(string city)
        {
            return await _context.Suppliers
                .Where(s => EF.Functions.ILike(s.City, $"%{city}%")) // Like query, case-insensitive
                .ToListAsync();
        }

        // Zoeken op land (like filter)
        public virtual async Task<List<Supplier>> SearchSuppliersByCountryAsync(string country)
        {
            return await _context.Suppliers
                .Where(s => EF.Functions.ILike(s.Country, $"%{country}%")) // Like query, case-insensitive
                .ToListAsync();
        }

        // Filteren op aanmaakdatums (tussen start- en einddatum)
        public virtual async Task<List<Supplier>> GetSuppliersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Suppliers
                .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
                .ToListAsync();
        }

        // Controle op duplicaat leveranciers
        public virtual async Task<bool> CheckDuplicateSupplierAsync(Supplier supplier)
        {
            return await _context.Suppliers.AnyAsync(s =>
                s.Name == supplier.Name &&
                s.City == supplier.City &&
                s.Country == supplier.Country);
        }

        // Verwijderen van meerdere leveranciers tegelijk
        public virtual async Task<bool> DeleteSuppliersBatchAsync(List<int> ids)
        {
            var suppliers = await _context.Suppliers
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();

            if (suppliers.Count != ids.Count)
            {
                return false; // Niet alle opgegeven leveranciers zijn gevonden
            }

            foreach (var supplier in suppliers)
            {
                supplier.IsDeleted = true;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        // Ophalen van totaal aantal leveranciers
        public virtual async Task<int> GetSupplierCountAsync()
        {
            return await _context.Suppliers.CountAsync();
        }

        public virtual async Task<Supplier> AddSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }
    }
}
