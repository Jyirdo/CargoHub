using CargohubV2.Controllers;
using CargohubV2.Models;
using CargohubV2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoHubTest
{
    [TestClass]
    public class OrdersControllerTests
    {
        private Mock<OrderService> _mockOrderService;
        private OrdersController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockOrderService = new Mock<OrderService>(null); // Pass null for DbContext
            _controller = new OrdersController(_mockOrderService.Object);
        }

        // Test GetAllOrders
        [TestMethod]
        public async Task GetAllOrders_ReturnsOk_WithOrders()
        {
            var orders = new List<Order>
            {
                new Order { Id = 1, Reference = "TestOrder1" },
                new Order { Id = 2, Reference = "TestOrder2" }
            };
            _mockOrderService.Setup(s => s.GetAllOrdersAsync()).ReturnsAsync(orders);

            var result = await _controller.GetAllOrders();

            var okResult = Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult)) as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedOrders = okResult.Value as List<Order>;
            Assert.AreEqual(2, returnedOrders.Count);
        }

        [TestMethod]
        public async Task GetAllOrders_ReturnsOk_WithEmptyList()
        {
            var orders = new List<Order>();
            _mockOrderService.Setup(s => s.GetAllOrdersAsync()).ReturnsAsync(orders);

            var result = await _controller.GetAllOrders();

            var okResult = Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult)) as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedOrders = okResult.Value as List<Order>;
            Assert.AreEqual(0, returnedOrders.Count);
        }

        // Test GetOrderById
        [TestMethod]
        public async Task GetOrderById_ReturnsOk_WhenOrderExists()
        {
            var orderId = 1;
            var order = new Order { Id = orderId, Reference = "TestOrder" };
            _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

            var result = await _controller.GetOrderById(orderId);

            var okResult = Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult)) as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedOrder = okResult.Value as Order;
            Assert.AreEqual(orderId, returnedOrder.Id);
        }

        [TestMethod]
        public async Task GetOrderById_ReturnsNoContent_WhenOrderNotFound()
        {
            var orderId = 1;
            _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync((Order)null);

            var result = await _controller.GetOrderById(orderId);

            Assert.IsInstanceOfType(result.Result, typeof(NoContentResult));
        }

        // Test AddOrder
        [TestMethod]
        public async Task AddOrder_ReturnsCreatedAtAction_WhenOrderIsAdded()
        {
            var newOrder = new Order { Id = 1, Reference = "NewOrder" };
            _mockOrderService.Setup(s => s.GetOrderByIdAsync(newOrder.Id)).ReturnsAsync((Order)null);
            _mockOrderService.Setup(s => s.AddOrderAsync(newOrder)).ReturnsAsync(newOrder);

            var result = await _controller.AddOrder(newOrder);

            var createdResult = Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult)) as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            var returnedOrder = createdResult.Value as Order;
            Assert.AreEqual(newOrder.Id, returnedOrder.Id);
        }

        [TestMethod]
        public async Task AddOrder_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            var newOrder = new Order(); // Missing required fields
            _controller.ModelState.AddModelError("Reference", "The Reference field is required.");

            var result = await _controller.AddOrder(newOrder);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task AddOrder_ReturnsBadRequest_WhenOrderAlreadyExists()
        {
            var newOrder = new Order { Id = 1, Reference = "DuplicateOrder" };
            _mockOrderService.Setup(s => s.GetOrderByIdAsync(newOrder.Id)).ReturnsAsync(newOrder);

            var result = await _controller.AddOrder(newOrder);

            var badRequestResult = Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult)) as BadRequestObjectResult;
            Assert.AreEqual("Order already exists", badRequestResult.Value);
        }

        // Test UpdateOrder
        [TestMethod]
        public async Task UpdateOrder_ReturnsNoContent_WhenOrderIsUpdated()
        {
            var orderId = 1;
            var updatedOrder = new Order { Id = orderId, Reference = "UpdatedOrder" };
            _mockOrderService.Setup(s => s.UpdateOrderAsync(orderId, updatedOrder)).ReturnsAsync(true);

            var result = await _controller.UpdateOrder(orderId, updatedOrder);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdateOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            var orderId = 1;
            var updatedOrder = new Order { Id = orderId, Reference = "UpdatedOrder" };
            _mockOrderService.Setup(s => s.UpdateOrderAsync(orderId, updatedOrder)).ReturnsAsync(false);

            var result = await _controller.UpdateOrder(orderId, updatedOrder);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        // Test DeleteOrder
        [TestMethod]
        public async Task DeleteOrder_ReturnsOk_WhenOrderIsDeleted()
        {
            var orderId = 1;
            _mockOrderService.Setup(s => s.DeleteOrderAsync(orderId)).ReturnsAsync(true);

            var result = await _controller.DeleteOrder(orderId);

            var okResult = Assert.IsInstanceOfType(result, typeof(OkResult));
            Assert.IsNotNull(okResult);
        }

        [TestMethod]
        public async Task DeleteOrder_ReturnsNoContent_WhenOrderNotFound()
        {
            var orderId = 1;
            _mockOrderService.Setup(s => s.DeleteOrderAsync(orderId)).ReturnsAsync(false);

            var result = await _controller.DeleteOrder(orderId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
