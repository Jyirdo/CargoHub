using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cargohub_V2.Models;
using Cargohub_V2.Services;

namespace Cargohub_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : Controller
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }

        // Ophalen van alle locaties (max 100)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> GetAllLocations()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return Ok(locations);
        }

        // Ophalen van een locatie via ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> GetLocationById(int id)
        {
            var location = await _locationService.GetLocationByIdAsync(id);
            if (location == null)
            {
                return NotFound(new { Message = $"Location with ID {id} not found." });
            }
            return Ok(location);
        }

        // Zoeken op naam (like filter)
        [HttpGet("Search/Name/{name}")]
        public async Task<ActionResult<IEnumerable<Location>>> SearchByName(string name)
        {
            var locations = await _locationService.SearchLocationsByNameAsync(name);
            return Ok(locations);
        }

        // Zoeken op code (exacte match)
        [HttpGet("Search/Code/{code}")]
        public async Task<ActionResult<Location>> SearchByCode(string code)
        {
            var location = await _locationService.SearchLocationByCodeAsync(code);
            if (location == null)
            {
                return NotFound(new { Message = $"Location with Code '{code}' not found." });
            }
            return Ok(location);
        }

        // Filteren op magazijn ID
        [HttpGet("Warehouse/{warehouseId}")]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocationsByWarehouseId(int warehouseId)
        {
            var locations = await _locationService.GetLocationsByWarehouseIdAsync(warehouseId);
            return Ok(locations);
        }

        // POST: Voeg een nieuwe locatie toe
        [HttpPost("Add")]
        public async Task<ActionResult<Location>> AddLocation([FromBody] Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdLocation = await _locationService.AddLocationAsync(location);
            return CreatedAtAction(nameof(GetLocationById), new { id = createdLocation.Id }, createdLocation);
        }

        // PUT: Update een bestaande locatie
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] Location updatedLocation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _locationService.UpdateLocationAsync(id, updatedLocation);
            if (updated == null)
            {
                return NotFound(new { Message = $"Location with ID {id} not found." });
            }
            return Ok(updated);
        }

        // DELETE: Verwijder een locatie via ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocationById(int id)
        {
            var deleted = await _locationService.DeleteLocationByIdAsync(id);
            if (!deleted)
            {
                return NotFound(new { Message = $"Location with ID {id} not found." });
            }
            return NoContent();
        }

        // Ophalen van totaal aantal locaties
        [HttpGet("Count")]
        public async Task<ActionResult<int>> GetLocationCount()
        {
            var count = await _locationService.GetLocationCountAsync();
            return Ok(count);
        }
    }
}
