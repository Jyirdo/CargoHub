using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cargohub_V2.Models;
using Cargohub_V2.Services;

namespace Cargohub_V2.Controllers
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetAllWarehouses()
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            return Ok(warehouses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouseById(int id)
        {
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
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedWarehouse = await _warehouseService.UpdateWarehouseAsync(id, warehouse);
            if (updatedWarehouse == null) return NotFound($"Warehouse with ID {id} not found.");

            return Ok(updatedWarehouse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouseById(int id)
        {
            var deleted = await _warehouseService.DeleteWarehouseByIdAsync(id);
            if (!deleted) return NotFound($"Warehouse with ID {id} not found.");

            return NoContent();
        }
    }
}
