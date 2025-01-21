using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using CargohubV2.Controllers;
using CargohubV2.Models;
using CargohubV2.Services;

namespace CargohubV2.Tests
{
    public class OrdersControllerTests
    {
        private readonly Mock<OrderService> _mockService;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _mockService = new Mock<OrderService>(null); // Mocked service
            _controller = new OrdersController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResultWithOrders()
        {
            // Arrange
            var mockOrders = new List<Order> { new Order { Id = 1, Reference = "Order1" } };
            _mockService.Setup(service => service.GetAllAsync()).ReturnsAsync(mockOrders);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrders = Xunit.Assert.IsType<List<Order>>(okResult.Value);
            Xunit.Assert.Single(returnedOrders);
        }


        [Fact]
        public async Task GetAllOrders_ValidAmount_ReturnsOkResultWithOrders()
        {
            // Arrange
            var mockOrders = new List<Order> { new Order { Id = 1, Reference = "Order1" } };
            _mockService.Setup(service => service.GetAllOrdersAsync(5)).ReturnsAsync(mockOrders);

            // Act
            var result = await _controller.GetAllOrders(5);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrders = Xunit.Assert.IsType<List<Order>>(okResult.Value);
            Xunit.Assert.Single(returnedOrders);
        }

        [Fact]
        public async Task GetOrderById_ValidId_ReturnsOkResultWithOrder()
        {
            // Arrange
            var mockOrder = new Order { Id = 1, Reference = "Order1" };
            _mockService.Setup(service => service.GetOrderByIdAsync(1)).ReturnsAsync(mockOrder);

            // Act
            var result = await _controller.GetOrderById(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrder = Xunit.Assert.IsType<Order>(okResult.Value);
            Xunit.Assert.Equal(mockOrder.Id, returnedOrder.Id);
        }

        [Fact]
        public async Task GetOrderById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetOrderById(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid order ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetItemsInOrder_ValidId_ReturnsOkResultWithItems()
        {
            // Arrange
            var mockItems = new List<OrderStock> { new OrderStock { Id = 1, Quantity = 10 } };
            _mockService.Setup(service => service.GetItemsInOrderAsync(1)).ReturnsAsync(mockItems);

            // Act
            var result = await _controller.GetItemsInOrder(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedItems = Xunit.Assert.IsType<List<OrderStock>>(okResult.Value);
            Xunit.Assert.Single(returnedItems);
        }

        [Fact]
        public async Task GetOrdersForShipment_ValidShipmentId_ReturnsOkResultWithOrders()
        {
            // Arrange
            var mockOrders = new List<Order> { new Order { Id = 1, ShipmentId = 10 } };
            _mockService.Setup(service => service.GetOrdersForShipmentAsync(10)).ReturnsAsync(mockOrders);

            // Act
            var result = await _controller.GetOrdersForShipment(10);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrders = Xunit.Assert.IsType<List<Order>>(okResult.Value);
            Xunit.Assert.Single(returnedOrders);
        }

        [Fact]
        public async Task AddOrder_ValidOrder_ReturnsCreatedAtAction()
        {
            // Arrange
            var newOrder = new Order { Id = 1, Reference = "Order1" };
            _mockService.Setup(service => service.AddOrderAsync(newOrder)).ReturnsAsync(newOrder);

            // Act
            var result = await _controller.AddOrder(newOrder);

            // Assert
            var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(result);
            var returnedOrder = Xunit.Assert.IsType<Order>(createdAtActionResult.Value);
            Xunit.Assert.Equal(newOrder.Id, returnedOrder.Id);
        }

        [Fact]
        public async Task UpdateOrder_ValidId_ReturnsNoContent()
        {
            // Arrange
            var updatedOrder = new Order { Id = 1, Reference = "UpdatedOrder" };
            _mockService.Setup(service => service.UpdateOrderAsync(1, updatedOrder)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateOrder(1, updatedOrder);

            // Assert
            Xunit.Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrder_ValidId_ReturnsOkResultWithSuccessMessage()
        {
            // Arrange
            _mockService.Setup(service => service.DeleteOrderAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteOrder(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Contains("Order deleted successfully", okResult.Value.ToString());
        }

        // Extra Test 1: GetOrdersForClient_ValidClientId_ReturnsOkResultWithOrders
        [Fact]
        public async Task GetOrdersForClient_ValidClientId_ReturnsOkResultWithOrders()
        {
            // Arrange
            var mockOrders = new List<Order> { new Order { Id = 1, ShipTo = "Client1" } };
            _mockService.Setup(service => service.GetOrdersForClientAsync("Client1")).ReturnsAsync(mockOrders);

            // Act
            var result = await _controller.GetOrdersForClient("Client1");

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrders = Xunit.Assert.IsType<List<Order>>(okResult.Value);
            Xunit.Assert.Single(returnedOrders);
        }

        // Extra Test 2: GetAllOrders_InvalidAmount_ReturnsBadRequest
        [Fact]
        public async Task GetAllOrders_InvalidAmount_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetAllOrders(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid amount", badRequestResult.Value.ToString());
        }
    }
}