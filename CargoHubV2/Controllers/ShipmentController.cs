using CargohubV2.Models;
using CargohubV2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CargohubV2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        private readonly ShipmentService _shipmentService;

        public ShipmentsController(ShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        [HttpGet("byAmount/{amount}")]
        public async Task<ActionResult<IEnumerable<Shipment>>> GetAllShipmentsByAmount(int amount)
        {
            if (amount <= 0)
            {
                return BadRequest(new { Message = "Invalid amount. It must be a positive integer." });
            }
            var shipments = await _shipmentService.GetAllShipmentsByAmountAsync(amount);
            return Ok(shipments);
        }

        [HttpGet("{shipmentId}")]
        public async Task<ActionResult<Shipment>> GetShipmentById(int shipmentId)
        {
            if (shipmentId <= 0)
            {
                return BadRequest(new { Message = "Invalid shipment ID. It must be a positive integer." });
            }
            var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
            if (shipment == null)
            {
                return NotFound(new { Message = $"Shipment with ID {shipmentId} not found." });
            }
            return Ok(shipment);
        }

        [HttpGet("{shipmentId}/items")]
        public async Task<ActionResult<List<ShipmentStock>>> GetItemsInShipment(int shipmentId)
        {
            if (shipmentId <= 0)
            {
                return BadRequest(new { Message = "Invalid shipment ID. It must be a positive integer." });
            }
            var items = await _shipmentService.GetItemsInShipmentAsync(shipmentId);
            if (items == null || !items.Any())
            {
                return NotFound(new { Message = $"No items found for shipment ID {shipmentId}." });
            }
            return Ok(items);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddShipment([FromBody] Shipment newShipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingShipment = await _shipmentService.GetShipmentByIdAsync(newShipment.Id);
            if (existingShipment != null)
            {
                return BadRequest("Shipment already exists");
            }
            var createdShipment = await _shipmentService.AddShipmentAsync(newShipment);
            return CreatedAtAction(nameof(GetShipmentById), new { shipmentId = createdShipment.Id }, createdShipment);
        }

        [HttpPut("{shipmentId}")]
        public async Task<IActionResult> UpdateShipment(int shipmentId, [FromBody] Shipment updatedShipment)
        {
            if (shipmentId <= 0)
            {
                return BadRequest(new { Message = "Invalid shipment ID. It must be a positive integer." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var UpdatedShipment = await _shipmentService.UpdateShipmentAsync(shipmentId, updatedShipment);
            if (UpdatedShipment == null)
            {
                return NoContent();
            }
            return Ok(UpdatedShipment);
        }

        [HttpPut("{shipmentId}/items")]
        public async Task<IActionResult> UpdateItemsInShipment(int shipmentId, [FromBody] List<ShipmentStock> updatedItems)
        {
            if (shipmentId <= 0)
            {
                return BadRequest(new { Message = "Invalid shipment ID. It must be a positive integer." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var UpdatedShipment = await _shipmentService.UpdateItemsInShipmentAsync(shipmentId, updatedItems);
            if (UpdatedShipment == null)
            {
                return NoContent();
            }
            return Ok(UpdatedShipment);
        }

        [HttpDelete("{shipmentId}")]
        public async Task<IActionResult> RemoveShipment(int shipmentId)
        {
            if (shipmentId <= 0)
            {
                return BadRequest(new { Message = "Invalid shipment ID. It must be a positive integer." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var shipment = await _shipmentService.RemoveShipmentAsync(shipmentId);
            if (shipment == null)
            {
                return NoContent();
            }
            return Ok("Shipment deleted successfully");
        }
    }
}