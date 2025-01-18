using CargohubV2.Models;
using CargohubV2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CargohubV2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/Orders
        [HttpGet("byAmount/{amount}")]
        public async Task<ActionResult<List<Order>>> GetAllOrders(int amount)
        {
            var orders = await _orderService.GetAllOrdersAsync(amount);
            return Ok(orders);
        }

        // GET: api/Orders/{id}
        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetOrderById(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NoContent();
            }
            return Ok(order);
        }

        [HttpGet("{orderId}/items")]
        public async Task<ActionResult<List<OrderStock>>> GetItemsInOrder(int orderId)
        {
            var items = await _orderService.GetItemsInOrderAsync(orderId);
            return Ok(items);
        }

        [HttpGet("shipment/{shipmentId}")]
        public async Task<ActionResult<List<Order>>> GetOrdersForShipment(int shipmentId)
        {
            var orders = await _orderService.GetOrdersForShipmentAsync(shipmentId);
            return Ok(orders);
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<List<Order>>> GetOrdersForClient(string clientId)
        {
            var orders = await _orderService.GetOrdersForClientAsync(clientId);
            return Ok(orders);
        }

        // POST: api/Orders/Add
        [HttpPost("Add")]
        public async Task<IActionResult> AddOrder([FromBody] Order newOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_orderService.GetOrderByIdAsync(newOrder.Id).Result != null)
            {
                return BadRequest("Order already exists");
            }
            var createdOrder = await _orderService.AddOrderAsync(newOrder);
            return CreatedAtAction(nameof(GetOrderById), new { orderId = createdOrder.Id }, createdOrder);
        }

        //PUT: api/Clients/Update/{id}
        [HttpPut("Update/{orderId}")] // Route parameter
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] Order updatedOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var orders = await _orderService.UpdateOrderAsync(orderId, updatedOrder);
            if (!orders)
            {
                return NotFound();
            }
            return NoContent();
        }


        // DELETE: api/Orders/Delete/{id}        
        [HttpDelete("Delete/{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var orders = await _orderService.DeleteOrderAsync(orderId);
            if (orders == null)
            {
                return NoContent();
            }
            return Ok(orders);
        }
    }
}