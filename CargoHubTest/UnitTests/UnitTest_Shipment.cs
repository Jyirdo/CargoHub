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
    public class ShipmentsControllerTests
    {
        private readonly Mock<ShipmentService> _mockService;
        private readonly ShipmentsController _controller;

        public ShipmentsControllerTests()
        {
            _mockService = new Mock<ShipmentService>(null); // Mocked service
            _controller = new ShipmentsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllShipmentsByAmount_ValidAmount_ReturnsOkResultWithShipments()
        {
            // Arrange
            var mockShipments = new List<Shipment>
            {
                new Shipment { Id = 1, OrderId = 1001, ShipmentStatus = "Pending" }
            };
            _mockService.Setup(service => service.GetAllShipmentsByAmountAsync(5)).ReturnsAsync(mockShipments);

            // Act
            var actionResult = await _controller.GetAllShipmentsByAmount(5);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedShipments = Xunit.Assert.IsType<List<Shipment>>(okResult.Value);
            Xunit.Assert.Single(returnedShipments);
        }

        [Fact]
        public async Task GetAllShipmentsByAmount_InvalidAmount_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetAllShipmentsByAmount(0);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Xunit.Assert.Contains("Invalid amount", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetShipmentById_ValidId_ReturnsOkResultWithShipment()
        {
            // Arrange
            var mockShipment = new Shipment { Id = 1, OrderId = 1001, ShipmentStatus = "Delivered" };
            _mockService.Setup(service => service.GetShipmentByIdAsync(1)).ReturnsAsync(mockShipment);

            // Act
            var actionResult = await _controller.GetShipmentById(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedShipment = Xunit.Assert.IsType<Shipment>(okResult.Value);
            Xunit.Assert.Equal(mockShipment.Id, returnedShipment.Id);
        }

        [Fact]
        public async Task GetShipmentById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetShipmentById(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Xunit.Assert.Contains("Invalid shipment ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetItemsInShipment_ValidId_ReturnsOkResultWithItems()
        {
            // Arrange
            var mockItems = new List<ShipmentStock>
            {
                new ShipmentStock { ItemId = "Item001", Quantity = 10 }
            };
            _mockService.Setup(service => service.GetItemsInShipmentAsync(1)).ReturnsAsync(mockItems);

            // Act
            var actionResult = await _controller.GetItemsInShipment(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedItems = Xunit.Assert.IsType<List<ShipmentStock>>(okResult.Value);
            Xunit.Assert.Single(returnedItems);
        }

        [Fact]
        public async Task AddShipment_ValidShipment_ReturnsCreatedAtAction()
        {
            // Arrange
            var newShipment = new Shipment { Id = 1, OrderId = 1001, ShipmentStatus = "Pending" };
            _mockService.Setup(service => service.AddShipmentAsync(newShipment)).ReturnsAsync(newShipment);

            // Act
            var actionResult = await _controller.AddShipment(newShipment);

            // Assert
            var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(actionResult);
            var returnedShipment = Xunit.Assert.IsType<Shipment>(createdAtActionResult.Value);
            Xunit.Assert.Equal(newShipment.Id, returnedShipment.Id);
        }

        [Fact]
        public async Task UpdateShipment_ValidId_ReturnsOkResultWithUpdatedShipment()
        {
            // Arrange
            var updatedShipment = new Shipment { Id = 1, OrderId = 1001, ShipmentStatus = "Delivered" };
            _mockService.Setup(service => service.UpdateShipmentAsync(1, updatedShipment)).ReturnsAsync(true);

            // Act
            var actionResult = await _controller.UpdateShipment(1, updatedShipment);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult);
            Xunit.Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task UpdateShipment_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.UpdateShipment(-1, new Shipment());

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            Xunit.Assert.Contains("Invalid shipment ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteShipment_ValidId_ReturnsOkResult()
        {
            // Arrange
            _mockService.Setup(service => service.RemoveShipmentAsync(1)).ReturnsAsync(true);

            // Act
            var actionResult = await _controller.RemoveShipment(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult);
            Xunit.Assert.Contains("Shipment deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteShipment_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.RemoveShipment(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            Xunit.Assert.Contains("Invalid shipment ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteShipment_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.RemoveShipmentAsync(99)).ReturnsAsync(false);

            // Act
            var actionResult = await _controller.RemoveShipment(99);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(actionResult); // Controleer type
            Xunit.Assert.Contains("Shipment with ID 99 not found", notFoundResult.Value.ToString()); // Controleer foutbericht
        }

    }
}