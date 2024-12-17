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

        [HttpGet]
        public async Task<ActionResult<List<Shipment>>> GetAllShipments()
        {
            var shipments = await _shipmentService.GetAllShipmentsAsync();
            return Ok(shipments);
        }

        [HttpGet("{shipmentId}")]
        public async Task<ActionResult<Shipment>> GetShipmentById(int shipmentId)
        {
            var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
            if (shipment == null)
            {
                return NotFound();
            }
            return Ok(shipment);
        }

        [HttpGet("{shipmentId}/items")]
        public async Task<ActionResult<List<ShipmentStock>>> GetItemsInShipment(int shipmentId)
        {
            var items = await _shipmentService.GetItemsInShipmentAsync(shipmentId);
            return Ok(items);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddShipment([FromBody] Shipment newShipment)
        {
            var createdShipment = await _shipmentService.AddShipmentAsync(newShipment);
            return CreatedAtAction(nameof(GetShipmentById), new { shipmentId = createdShipment.Id }, createdShipment);
        }

        [HttpPut("{shipmentId}")]
        public async Task<IActionResult> UpdateShipment(int shipmentId, [FromBody] Shipment updatedShipment)
        {
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var shipment = await _shipmentService.RemoveShipmentAsync(shipmentId);
            if (shipment == null)
            {
                return NoContent();
            }
            return Ok(shipment);
        }
    }
}