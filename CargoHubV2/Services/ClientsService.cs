using System.Collections.Generic;
using System.Threading.Tasks;
using CargohubV2.Contexts;
using CargohubV2.Models;
using Microsoft.EntityFrameworkCore;

namespace CargohubV2.Services
{
    public class ClientsService
    {
        private readonly CargoHubDbContext _context;

        // Constructor to inject CargoHubDbContext
        public ClientsService(CargoHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<Client>> GetAllClientsAsync(int amount)
        {
            return await _context.Clients.Take(amount).ToListAsync();
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            var possibleClient = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (possibleClient != null)
            {
                return possibleClient;
            }
            return null;
        }

        public async Task<Client> GetClientByEmailAsync(string email)
        {
            var possibleClient = await _context.Clients.FirstOrDefaultAsync(c => c.ContactEmail == email);
            if (possibleClient != null)
            {
                return possibleClient;
            }
            return null;
        }

        public async Task<Client> CreateClientAsync(Client client)
        {
            DateTime CreatedAt = DateTime.UtcNow;
            DateTime UpdatedAt = DateTime.UtcNow;

            client.CreatedAt = new DateTime(CreatedAt.Year, CreatedAt.Month, CreatedAt.Day, CreatedAt.Hour, CreatedAt.Minute, CreatedAt.Second, DateTimeKind.Utc);
            client.UpdatedAt = new DateTime(UpdatedAt.Year, UpdatedAt.Month, UpdatedAt.Day, UpdatedAt.Hour, UpdatedAt.Minute, UpdatedAt.Second, DateTimeKind.Utc);

            // Add the new client
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return client;
        }

        public async Task<Client> UpdateClientAsync(Client client, int id)
        {
            var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);

            if (existingClient == null)
            {
                return null;
            }

            existingClient.Name = client.Name;
            existingClient.Address = client.Address;
            existingClient.City = client.City;
            existingClient.ZipCode = client.ZipCode;
            existingClient.Province = client.Province;
            existingClient.Country = client.Country;
            existingClient.ContactName = client.ContactName;
            existingClient.ContactPhone = client.ContactPhone;
            existingClient.ContactEmail = client.ContactEmail;

            DateTime UpdatedAt = DateTime.UtcNow;
            existingClient.UpdatedAt = new DateTime(UpdatedAt.Year, UpdatedAt.Month, UpdatedAt.Day, UpdatedAt.Hour, UpdatedAt.Minute, UpdatedAt.Second, DateTimeKind.Utc);

            _context.Clients.Update(existingClient);
            await _context.SaveChangesAsync();

            return existingClient;
        }

        public async Task<Client> RemoveClientByIdAsync(int id)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
            {
                return null;
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return client;
        }
        public async Task<Client> RemoveClientByEmailAsync(string email)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ContactEmail == email);

            if (client == null)
            {
                return null;
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return client;
        }

        internal object Take(int v)
        {
            throw new NotImplementedException();
        }
    }
}
