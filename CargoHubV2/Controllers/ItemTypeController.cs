using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CargohubV2.Models;
using CargohubV2.Services;

namespace CargohubV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemTypesController : Controller
    {
        private readonly ItemTypeService _itemTypeService;

        public ItemTypesController(ItemTypeService itemTypeService)
        {
            _itemTypeService = itemTypeService;
        }

        // GET: api/ItemTypes
        [HttpGet("byAmount/{amount}")]
        public async Task<ActionResult<IEnumerable<Item_Type>>> GetAllItemTypes(int amount)
        {
            if (amount <= 0)
            {
                return BadRequest(new { Message = "Invalid amount. It must be a positive integer." });
            }
            var itemTypes = await _itemTypeService.GetAllItemTypesAsync(amount);
            return Ok(itemTypes);
        }

        // GET: api/ItemTypes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Item_Type>> GetItemTypeById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid itemtype ID. It must be a positive integer." });
            }

            var itemType = await _itemTypeService.GetItemTypeByIdAsync(id);

            if (itemType == null)
            {
                return NotFound(new { Message = $"Item type with ID {id} not found." });
            }

            return Ok(itemType);
        }

        [HttpGet("ByName/{name}")] // Route parameter
        public async Task<ActionResult<Item_Type>> GetItemTypeByName(string name)
        {
            var itemType = await _itemTypeService.GetItemTypeByNameAsync(name);
            if (itemType == null)
            {
                return NotFound(new { Message = $"Item type with Name: {name} not found." });
            }
            return Ok(itemType);
        }

        // POST: api/ItemTypes
        [HttpPost("Add")]
        public async Task<ActionResult<Item_Type>> AddItemType([FromBody] Item_Type newItemType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_itemTypeService.GetItemTypeByNameAsync(newItemType.Name).Result != null)
            {
                return BadRequest("Item Type with this name already exists");
            }

            var createdItemType = await _itemTypeService.AddItemTypeAsync(newItemType);
            return CreatedAtAction(nameof(GetItemTypeById), new { id = createdItemType.Id }, createdItemType);
        }

        // PUT: api/ItemTypes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemType(int id, [FromBody] Item_Type itemType)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid itemtype ID. It must be a positive integer." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedItemLine = await _itemTypeService.UpdateItemTypeAsync(id, itemType);
            if (updatedItemLine == null)
            {
                return NoContent();
            }
            return Ok(updatedItemLine);
        }

        // DELETE: api/ItemTypes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemType(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid itemtype ID. It must be a positive integer." });
            }

            var success = await _itemTypeService.DeleteItemTypeAsync(id);

            if (!success)
            {
                return NotFound(new { Message = $"Item type with ID {id} not found." });
            }

            return Ok("Item type deleted successfully");
        }
    }
}
