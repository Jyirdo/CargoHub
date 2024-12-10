using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub_V2.Contexts;
using Cargohub_V2.Models;
using Microsoft.EntityFrameworkCore;

namespace Cargohub_V2.Services
{
    public class LocationService
    {
        private readonly CargoHubDbContext _context;

        public LocationService(CargoHubDbContext context)
        {
            _context = context;
        }

        // Ophalen van alle locaties (max 100)
        public async Task<List<Location>> GetAllLocationsAsync()
        {
            return await _context.Locations
                .OrderBy(l => l.Id)
                .Take(100)
                .ToListAsync();
        }

        // Ophalen van een locatie via ID
        public async Task<Location> GetLocationByIdAsync(int id)
        {
            return await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);
        }

        // Zoeken op naam (like filter)
        public async Task<List<Location>> SearchLocationsByNameAsync(string name)
        {
            return await _context.Locations
                .Where(l => EF.Functions.ILike(l.Name, $"%{name}%"))
                .ToListAsync();
        }

        // Zoeken op code (exacte match)
        public async Task<Location> SearchLocationByCodeAsync(string code)
        {
            return await _context.Locations.FirstOrDefaultAsync(l => l.Code == code);
        }

        // Filteren op magazijn ID
        public async Task<List<Location>> GetLocationsByWarehouseIdAsync(int warehouseId)
        {
            return await _context.Locations
                .Where(l => l.WarehouseId == warehouseId)
                .ToListAsync();
        }

        // Voeg een nieuwe locatie toe
        public async Task<Location> AddLocationAsync(Location location)
        {
            DateTime now = DateTime.UtcNow;

            location.CreatedAt = now;
            location.UpdatedAt = now;

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return location;
        }

        // Update een bestaande locatie
        public async Task<Location?> UpdateLocationAsync(int id, Location updatedLocation)
        {
            var existingLocation = await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);

            if (existingLocation == null)
            {
                return null; // Locatie niet gevonden
            }

            existingLocation.WarehouseId = updatedLocation.WarehouseId;
            existingLocation.Code = updatedLocation.Code;
            existingLocation.Name = updatedLocation.Name;
            existingLocation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingLocation;
        }

        // Verwijder een locatie via ID
        public async Task<bool> DeleteLocationByIdAsync(int id)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);

            if (location == null)
            {
                return false;
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return true;
        }

        // Ophalen van totaal aantal locaties
        public async Task<int> GetLocationCountAsync()
        {
            return await _context.Locations.CountAsync();
        }
    }
}
