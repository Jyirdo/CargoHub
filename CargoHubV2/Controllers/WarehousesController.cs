using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CargohubV2.Models;
using CargohubV2.Services;

namespace CargohubV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : Controller
    {
        private readonly WarehouseService _warehouseService;

        public WarehousesController(WarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet("byAmount/{amount}")]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetAllWarehouses(int amount)
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync(amount);
            return Ok(warehouses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouseById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid warehouse ID. It must be a positive integer." });
            }
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null) return NotFound($"Warehouse with ID {id} not found.");
            return Ok(warehouse);
        }

        [HttpGet("ByCity/{city}")]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetWarehousesByCity(string city)
        {
            var warehouses = await _warehouseService.GetWarehousesByCityAsync(city);
            return Ok(warehouses);
        }

        [HttpPost]
        public async Task<ActionResult<Warehouse>> AddWarehouse([FromBody] Warehouse warehouse)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdWarehouse = await _warehouseService.AddWarehouseAsync(warehouse);
            return CreatedAtAction(nameof(GetWarehouseById), new { id = createdWarehouse.Id }, createdWarehouse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] Warehouse warehouse)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid warehouse ID. It must be a positive integer." });
            }
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedWarehouse = await _warehouseService.UpdateWarehouseAsync(id, warehouse);
            if (updatedWarehouse == null) return NotFound($"Warehouse with ID {id} not found.");

            return Ok(updatedWarehouse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouseById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid warehouse ID. It must be a positive integer." });
            }
            var deleted = await _warehouseService.DeleteWarehouseByIdAsync(id);
            if (!deleted) return NotFound($"Warehouse with ID {id} not found.");

            return Ok("Warehouse deleted successfully");
        }
    }
}
