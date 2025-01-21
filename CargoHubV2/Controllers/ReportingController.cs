using Microsoft.AspNetCore.Mvc;
using CargohubV2.Services;

namespace CargohubV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ReportingService _reportingService;

        // Inject the ReportingService
        public ReportsController(ReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        // GET: api/reports/warehouse/{warehouseId}/generate
        [HttpGet("warehouse/{warehouseId}/generate")]
        public ActionResult<ReportResult> GetWarehouseReport(int warehouseId)
        {
            try
            {
                // Get the report data
                var report = _reportingService.GetWarehouseReport(warehouseId);

                // Return the report data as a response
                return Ok(report);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return a 500 error with the message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/reports/warehouse/{warehouseId}/download
        [HttpGet("warehouse/{warehouseId}/download")]
        public IActionResult DownloadReport(int warehouseId)
        {
            try
            {
                // Generate the CSV report for the given warehouseId
                var csvContent = _reportingService.GenerateCsvReport(warehouseId);

                // Convert the CSV string content to a byte array
                var byteArray = System.Text.Encoding.UTF8.GetBytes(csvContent);

                // Return the CSV as a downloadable file
                return File(byteArray, "text/csv", $"Warehouse_{warehouseId}_Report.csv");
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return a 500 error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("warehouse/{warehouseId}/locations")]
        public ActionResult<LocationResult> GetLocations(int warehouseId)
        {
            try
            {
                // Get location data
                var locations = _reportingService.GetLocationsByWarehouseId(warehouseId);

                // Return as JSON
                return Ok(locations);
            }
            catch (Exception ex)
            {
                // Handle any errors
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
