using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cargohub_V2.Models;
using Cargohub_V2.Services;

namespace Cargohub_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransfersController : Controller
    {
        private readonly TransferService _transferService;

        public TransfersController(TransferService transferService)
        {
            _transferService = transferService;
        }

        // Get all transfers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transfer>>> GetAllTransfers()
        {
            var transfers = await _transferService.GetAllTransfersAsync();
            return Ok(transfers);
        }

        // Get Transfer Status by ID
        [HttpGet("Status/{id}")]
        public async Task<ActionResult> GetTransferStatusById(int id)
        {
            var transfer = await _transferService.GetTransferByIdAsync(id);
            if (transfer == null)
                return NotFound(new { Message = $"Transfer with ID {id} not found." });

            return Ok(new { id = transfer.Id, transferStatus = transfer.TransferStatus });
        }


        // Get a specific transfer by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Transfer>> GetTransferById(int id)
        {
            var transfer = await _transferService.GetTransferByIdAsync(id);
            if (transfer == null) return NotFound($"Transfer with ID {id} not found.");
            return Ok(transfer);
        }

        // Get transfers by status
        [HttpGet("ByStatus/{status}")]
        public async Task<ActionResult<IEnumerable<Transfer>>> GetTransfersByStatus(string status)
        {
            var transfers = await _transferService.GetTransfersByStatusAsync(status);
            return Ok(transfers);
        }

        // Add a new transfer
        [HttpPost]
        public async Task<ActionResult<Transfer>> AddTransfer([FromBody] Transfer transfer)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdTransfer = await _transferService.AddTransferAsync(transfer);
            return CreatedAtAction(nameof(GetTransferById), new { id = createdTransfer.Id }, createdTransfer);
        }

        // Update an existing transfer
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransfer(int id, [FromBody] Transfer transfer)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedTransfer = await _transferService.UpdateTransferAsync(id, transfer);
            if (updatedTransfer == null) return NotFound($"Transfer with ID {id} not found.");

            return Ok(updatedTransfer);
        }

        // Delete a transfer by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransferById(int id)
        {
            var deleted = await _transferService.DeleteTransferByIdAsync(id);
            if (!deleted) return NotFound($"Transfer with ID {id} not found.");

            return NoContent();
        }

        // Delete transfers by status
        [HttpDelete("ByStatus/{status}")]
        public async Task<IActionResult> DeleteTransfersByStatus(string status)
        {
            var deleted = await _transferService.DeleteTransfersByStatusAsync(status);
            if (!deleted) return NotFound($"No transfers found with status '{status}'.");

            return NoContent();
        }
    }
}
