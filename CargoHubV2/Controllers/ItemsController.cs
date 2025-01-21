using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CargohubV2.Models;
using CargohubV2.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CargohubV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : Controller
    {
        private readonly ItemService _itemService;

        public ItemsController(ItemService itemService)
        {
            _itemService = itemService;
        }

        // GET: api/Items
        [HttpGet("byAmount/{amount}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetAllItems(int amount)
        {
            if (amount <= 0)
            {
                return BadRequest(new { Message = "Invalid amount. It must be a positive integer." });
            }
            var items = await _itemService.GetAllItemsAsync(amount);
            return Ok(items);
        }

        // GET: api/Items/{id}
        [HttpGet("{uid}")]
        public async Task<ActionResult<Item>> GetItemById(string uid)
        {
            var item = await _itemService.GetItemByUidAsync(uid);

            if (item == null)
            {
                return NotFound(new { Message = $"Item with UID {uid} not found." });
            }

            return Ok(item);
        }

        // GET: api/Items/ByItemLine/{itemLineId}
        [HttpGet("ByItemLine/{itemLineId}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItemsByItemLine(int itemLineId)
        {
            if (itemLineId <= 0)
            {
                return BadRequest(new { Message = "Invalid itemsline ID. It must be a positive integer." });
            }
            var items = await _itemService.GetItemsByItemLineAsync(itemLineId);
            if (items == null)
            {
                return NotFound();
            }
            return Ok(items);
        }


        // GET: api/Items/ByItemGroup/{itemGroupId}
        [HttpGet("ByItemGroup/{itemGroupId}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItemsByItemGroup(int itemGroupId)
        {
            if (itemGroupId <= 0)
            {
                return BadRequest(new { Message = "Invalid itemsline ID. It must be a positive integer." });
            }
            var items = await _itemService.GetItemsByItemGroupAsync(itemGroupId);
            if (items == null)
            {
                return NotFound();
            }
            return Ok(items);
        }

        // GET: api/Items/ByItemType/{itemTypeId}
        [HttpGet("ByItemType/{itemTypeId}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItemsByItemType(int itemTypeId)
        {
            if (itemTypeId <= 0)
            {
                return BadRequest(new { Message = "Invalid itemsline ID. It must be a positive integer." });
            }
            var items = await _itemService.GetItemsByItemTypeAsync(itemTypeId);
            if (items == null)
            {
                return NotFound();
            }
            return Ok(items);
        }

        // GET: api/Items/BySupplier/{supplierId}
        [HttpGet("BySupplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItemsBySupplier(int supplierId)
        {
            if (supplierId <= 0)
            {
                return BadRequest(new { Message = "Invalid itemsline ID. It must be a positive integer." });
            }
            var items = await _itemService.GetItemsBySupplierAsync(supplierId);
            if (items == null)
            {
                return NotFound();
            }
            return Ok(items);
        }

        // POST: api/Items/PopulateWeightInKg
        [HttpPost("PopulateWeightInKg")]
        public async Task<IActionResult> PopulateWeightInKg()
        {
            await _itemService.PopulateWeightInKgAsync();
            return Ok("WeightInKg column populated with random values.");
        }

        // PUT: api/Items/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] Item updatedItem)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid itemsline ID. It must be a positive integer." });
            }
            if (id != updatedItem.Id)
            {
                return BadRequest(new { Message = "ID in the URL does not match the ID in the payload." });
            }

            var success = await _itemService.UpdateItemAsync(id, updatedItem);

            if (!success)
            {
                return NotFound(new { Message = $"Item with ID {id} not found." });
            }

            return NoContent();
        }

        // DELETE: api/Items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveItem(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid itemsline ID. It must be a positive integer." });
            }
            var success = await _itemService.RemoveItemAsync(id);

            if (!success)
            {
                return NotFound(new { Message = $"Item with ID {id} not found." });
            }

            return Ok("Item deleted successfully");
        }
    }
}
