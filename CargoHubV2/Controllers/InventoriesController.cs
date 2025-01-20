using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CargohubV2.Models;
using CargohubV2.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CargohubV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoriesController : Controller
    {
        private readonly InventoriesService _inventoriesService;

        public InventoriesController(InventoriesService inventoriesService)
        {
            _inventoriesService = inventoriesService;
        }

        // GET: api/inventories
        [HttpGet("byAmount/{amount}")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetAllInventories(int amount)
        {
            var inventories = await _inventoriesService.GetAllInventoriesAsync(amount);
            return Ok(inventories);
        }

        // GET: api/inventories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventoryById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid inventory ID. It must be a positive integer." });
            }

            var inventory = await _inventoriesService.GetInventoriesByIdAsync(id);

            if (inventory == null)
            {
                return NotFound(new { Message = $"Inventory with ID {id} not found." });
            }

            return Ok(inventory);
        }

        // POST: api/inventories/Add
        [HttpPost("Add")]
        public async Task<ActionResult<Inventory>> AddInventory([FromBody] Inventory newInventory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_inventoriesService.AddInventoryAsync(newInventory).Result != null)
            {
                return BadRequest("Inventory with this ID already exists");
            }
            var createdInventory = await _inventoriesService.AddInventoryAsync(newInventory);
            return CreatedAtAction(nameof(GetInventoryById), new { id = createdInventory.Id }, createdInventory);
        }

        // PUT: api/inventories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] Inventory updatedInventory)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid inventory ID. It must be a positive integer." });
            }

            if (id != updatedInventory.Id)
            {
                return BadRequest(new { Message = "ID in the URL does not match the ID in the payload." });
            }

            var success = await _inventoriesService.UpdateInventoryAsync(id, updatedInventory);

            if (!success)
            {
                return NotFound(new { Message = $"Inventory with ID {id} not found." });
            }

            return NoContent();
        }

        // DELETE: api/inventories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveInventory(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid inventory ID. It must be a positive integer." });
            }
            
            var success = await _inventoriesService.RemoveInventoryAsync(id);

            if (!success)
            {
                return NotFound(new { Message = $"Inventory with ID {id} not found." });
            }
            return Ok("Inventory deleted successfully");
        }
    }
}
