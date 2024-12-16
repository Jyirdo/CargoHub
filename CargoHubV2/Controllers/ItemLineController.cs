using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cargohub_V2.Models;
using Cargohub_V2.Services;

namespace Cargohub_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemLinesController : Controller
    {
        private readonly ItemLineService _itemLineService;

        public ItemLinesController(ItemLineService itemLineService)
        {
            _itemLineService = itemLineService;
        }

        // GET: api/ItemLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item_Line>>> GetAllItemLines()
        {
            var itemLines = await _itemLineService.GetAllItemLinesAsync();
            return Ok(itemLines);
        }

        // GET: api/ItemLines/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Item_Line>> GetItemLineById(int id)
        {
            var itemLine = await _itemLineService.GetItemLineByIdAsync(id);

            if (itemLine == null)
            {
                return NotFound(new { Message = $"Item line with ID {id} not found." });
            }

            return Ok(itemLine);
        }

        // GET: api/ItemLines/ByName/name
        [HttpGet("ByName/{name}")] // Route parameter
        public async Task<ActionResult<Item_Line>> GetItemLineByName(string name)
        {
            var itemLine = await _itemLineService.GetItemLineByNameAsync(name);
            if (itemLine == null)
            {
                return NotFound(new { Message = $"Item line with Name: {name} not found." });
            }
            return Ok(itemLine);
        }

        // POST: api/ItemLines/Add
        [HttpPost("Add")]
        public async Task<ActionResult<Item_Line>> AddItemLine([FromBody] Item_Line ItemLine)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_itemLineService.GetItemLineByNameAsync(ItemLine.Name).Result != null)
            {
                return BadRequest("Item Group with this name already exists");
            }
            var createdItemLine = await _itemLineService.AddItemLineAsync(ItemLine);
            return CreatedAtAction(nameof(GetItemLineById), new { id = createdItemLine.Id }, createdItemLine);
        }

        // PUT: api/ItemLines/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemLine(int id, [FromBody] Item_Line ItemLine)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedItemLine = await _itemLineService.UpdateItemLineAsync(id, ItemLine);
            if (updatedItemLine == null)
            {
                return NoContent();
            }

            return Ok(updatedItemLine);
        }

        // DELETE: api/ItemLines/{id}
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteItemLine(int id)
        {
            var success = await _itemLineService.DeleteItemLineAsync(id);

            if (!success)
            {
                return NotFound(new { Message = $"Item line with ID {id} not found." });
            }

            return Ok("Item group deleted successfully");
        }
    }
}
