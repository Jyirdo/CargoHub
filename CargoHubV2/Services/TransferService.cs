using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargohubV2.Contexts;
using CargohubV2.Models;
using Microsoft.EntityFrameworkCore;

namespace CargohubV2.Services
{
    public class TransferService
    {
        private readonly CargoHubDbContext _context;

        public TransferService(CargoHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transfer>> GetAllTransfersAsync(int amount)
        {
            return await _context.Transfers.Take(amount).ToListAsync();
        }

        public async Task<Transfer> GetTransferByIdAsync(int id)
        {
            return await _context.Transfers.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Transfer>> GetTransfersByStatusAsync(string status)
        {
            return await _context.Transfers.Where(t => t.TransferStatus == status).ToListAsync();
        }

        public async Task<Transfer> AddTransferAsync(Transfer transfer)
        {
            transfer.CreatedAt = DateTime.UtcNow;
            transfer.UpdatedAt = DateTime.UtcNow;

            _context.Transfers.Add(transfer);
            await _context.SaveChangesAsync();

            return transfer;
        }

        public async Task<Transfer> UpdateTransferAsync(int id, Transfer transfer)
        {
            var existingTransfer = await _context.Transfers.FindAsync(id);
            if (existingTransfer == null) return null;

            existingTransfer.Reference = transfer.Reference;
            existingTransfer.TransferFrom = transfer.TransferFrom;
            existingTransfer.TransferTo = transfer.TransferTo;
            existingTransfer.TransferStatus = transfer.TransferStatus;
            existingTransfer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return existingTransfer;
        }

        public async Task<bool> DeleteTransferByIdAsync(int id)
        {
            var transfer = await _context.Transfers.FindAsync(id);
            if (transfer == null) return false;

            transfer.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTransfersByStatusAsync(string status)
        {
            var transfers = await _context.Transfers.Where(t => t.TransferStatus == status).ToListAsync();
            if (!transfers.Any()) return false;

            _context.Transfers.RemoveRange(transfers);
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
