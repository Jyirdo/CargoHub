using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CargohubV2.Models;
using CargohubV2.Services;

namespace CargohubV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemGroupsController : Controller
    {
        private readonly ItemGroupService _itemGroupService;

        public ItemGroupsController(ItemGroupService itemGroupService)
        {
            _itemGroupService = itemGroupService;
        }

        // GET: api/ItemGroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item_Group>>> GetAllItemGroups()
        {
            var itemGroups = await _itemGroupService.GetAllItemGroupsAsync();
            return Ok(itemGroups);
        }

        // GET: api/ItemGroups/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Item_Group>> GetItemGroupById(int id)
        {
            var itemGroup = await _itemGroupService.GetItemGroupByIdAsync(id);

            if (itemGroup == null)
            {
                return NotFound(new { Message = $"Item group with ID {id} not found." });
            }

            return Ok(itemGroup);
        }

        // GET: api/ItemGroups/ByName/name
        [HttpGet("ByName/{name}")]
        public async Task<ActionResult<Item_Group>> GetItemGroupByName(string name)
        {
            var itemGroup = await _itemGroupService.GetItemGroupByNameAsync(name);

            if (itemGroup == null)
            {
                return NotFound(new { Message = $"Item group with Name: {name} not found." });
            }

            return Ok(itemGroup);
        }

        // POST: api/ItemGroups/Add
        [HttpPost("Add")]
        public async Task<ActionResult<Item_Group>> AddItemGroup([FromBody] Item_Group itemGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_itemGroupService.GetItemGroupByNameAsync(itemGroup.Name).Result != null)
            {
                return BadRequest("Item Group with this name already exists");
            }

            var createdItemGroup = await _itemGroupService.AddItemGroupAsync(itemGroup);
            return CreatedAtAction(nameof(GetItemGroupById), new { id = createdItemGroup.Id }, createdItemGroup);
        }

        // DELETE: api/ItemGroups/Delete/{id}
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteItemGroup(int id)
        {
            var success = await _itemGroupService.RemoveItemGroupAsync(id);

            if (!success)
            {
                return NotFound(new { Message = $"Item group with ID {id} not found." });
            }

            return Ok("Item group deleted successfully");
        }

        // PUT: api/ItemGroups/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemGroup(int id, [FromBody] Item_Group ItemGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedItemGroup = await _itemGroupService.UpdateItemGroupAsync(id, ItemGroup);
            if (updatedItemGroup == null)
            {
                return NoContent();
            }

            return Ok(updatedItemGroup);
        }
    }
}
