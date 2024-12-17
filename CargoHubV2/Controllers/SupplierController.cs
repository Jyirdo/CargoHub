using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CargohubV2.Models;
using CargohubV2.Services;

namespace CargohubV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : Controller
    {
        private readonly SupplierService _supplierService;

        public SupplierController(SupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // Ophalen van alle leveranciers (max 100)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAllSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        // Ophalen van een leverancier via ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplierById(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                return NotFound(new { Message = $"Supplier with ID {id} not found." });
            }
            return Ok(supplier);
        }

        // Zoeken op naam (like filter)
        [HttpGet("Search/Name/{name}")]
        public async Task<ActionResult<IEnumerable<Supplier>>> SearchByName(string name)
        {
            var suppliers = await _supplierService.SearchSuppliersByNameAsync(name);
            return Ok(suppliers);
        }

        // Zoeken op stad (like filter)
        [HttpGet("Search/City/{city}")]
        public async Task<ActionResult<IEnumerable<Supplier>>> SearchByCity(string city)
        {
            var suppliers = await _supplierService.SearchSuppliersByCityAsync(city);
            return Ok(suppliers);
        }

        // Zoeken op land (like filter)
        [HttpGet("Search/Country/{country}")]
        public async Task<ActionResult<IEnumerable<Supplier>>> SearchByCountry(string country)
        {
            var suppliers = await _supplierService.SearchSuppliersByCountryAsync(country);
            return Ok(suppliers);
        }

        // Filteren op aanmaakdatums (tussen start- en einddatum)
        [HttpGet("DateRange")]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliersByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var suppliers = await _supplierService.GetSuppliersByDateRangeAsync(startDate, endDate);
            return Ok(suppliers);
        }

        // Controle op duplicaat leveranciers
        [HttpPost("CheckDuplicate")]
        public async Task<ActionResult<bool>> CheckDuplicate([FromBody] Supplier supplier)
        {
            var isDuplicate = await _supplierService.CheckDuplicateSupplierAsync(supplier);
            return Ok(isDuplicate);
        }

        // Verwijderen van meerdere leveranciers tegelijk
        [HttpDelete("DeleteBatch")]
        public async Task<IActionResult> DeleteBatch([FromBody] List<int> ids)
        {
            var result = await _supplierService.DeleteSuppliersBatchAsync(ids);
            if (!result)
            {
                return NotFound(new { Message = "Some suppliers were not found." });
            }
            return NoContent();
        }

        // Ophalen van totaal aantal leveranciers
        [HttpGet("Count")]
        public async Task<ActionResult<int>> GetSupplierCount()
        {
            var count = await _supplierService.GetSupplierCountAsync();
            return Ok(count);
        }
    }
}
