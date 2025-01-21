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
    public class InventoriesControllerTests
    {
        private readonly Mock<InventoriesService> _mockService;
        private readonly InventoriesController _controller;

        public InventoriesControllerTests()
        {
            _mockService = new Mock<InventoriesService>(null); // Mock service
            _controller = new InventoriesController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllInventories_ValidAmount_ReturnsOkResultWithInventories()
        {
            // Arrange
            var mockInventories = new List<Inventory>
            {
                new Inventory { Id = 1, ItemId = "Item1", TotalOnHand = 10 }
            };
            _mockService.Setup(service => service.GetAllInventoriesAsync(5)).ReturnsAsync(mockInventories);

            // Act
            var actionResult = await _controller.GetAllInventories(5);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<IEnumerable<Inventory>>>(actionResult);
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedInventories = Xunit.Assert.IsType<List<Inventory>>(okResult.Value);
            Xunit.Assert.Single(returnedInventories);
        }

        [Fact]
        public async Task GetAllInventories_InvalidAmount_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetAllInventories(0);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<IEnumerable<Inventory>>>(actionResult);
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid amount", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetInventoryById_ValidId_ReturnsOkResultWithInventory()
        {
            // Arrange
            var mockInventory = new Inventory { Id = 1, ItemId = "Item1", TotalOnHand = 10 };
            _mockService.Setup(service => service.GetInventoriesByIdAsync(1)).ReturnsAsync(mockInventory);

            // Act
            var actionResult = await _controller.GetInventoryById(1);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Inventory>>(actionResult);
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedInventory = Xunit.Assert.IsType<Inventory>(okResult.Value);
            Xunit.Assert.Equal(mockInventory.Id, returnedInventory.Id);
        }

        [Fact]
        public async Task GetInventoryById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetInventoryById(-1);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Inventory>>(actionResult);
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid inventory ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetInventoryById_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.GetInventoriesByIdAsync(99)).ReturnsAsync((Inventory)null);

            // Act
            var actionResult = await _controller.GetInventoryById(99);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Inventory>>(actionResult);
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
            Xunit.Assert.Contains("Inventory with ID 99 not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task AddInventory_ValidInventory_ReturnsCreatedAtAction()
        {
            // Arrange
            var newInventory = new Inventory { Id = 1, ItemId = "Item1", TotalOnHand = 10 };
            _mockService.Setup(service => service.GetInventoriesByIdAsync(newInventory.Id)).ReturnsAsync((Inventory)null);
            _mockService.Setup(service => service.AddInventoryAsync(newInventory)).ReturnsAsync(newInventory);

            // Act
            var actionResult = await _controller.AddInventory(newInventory);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Inventory>>(actionResult);
            var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedInventory = Xunit.Assert.IsType<Inventory>(createdAtActionResult.Value);
            Xunit.Assert.Equal(newInventory.Id, returnedInventory.Id);
        }

        [Fact]
        public async Task AddInventory_InvalidInventory_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("ItemId", "The ItemId field is required.");

            // Act
            var actionResult = await _controller.AddInventory(new Inventory());

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var errors = badRequestResult.Value as SerializableError;
            Xunit.Assert.NotNull(errors);
            Xunit.Assert.True(errors.ContainsKey("ItemId"));
        }

        [Fact]
        public async Task UpdateInventory_ValidId_ReturnsNoContent()
        {
            // Arrange
            var updatedInventory = new Inventory { Id = 1, ItemId = "UpdatedItem", TotalOnHand = 20 };
            _mockService.Setup(service => service.UpdateInventoryAsync(1, updatedInventory)).ReturnsAsync(true);

            // Act
            var actionResult = await _controller.UpdateInventory(1, updatedInventory);

            // Assert
            Xunit.Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task UpdateInventory_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.UpdateInventory(-1, new Inventory());

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            Xunit.Assert.Contains("Invalid inventory ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task RemoveInventory_ValidId_ReturnsOkResult()
        {
            // Arrange
            _mockService.Setup(service => service.RemoveInventoryAsync(1)).ReturnsAsync(true);

            // Act
            var actionResult = await _controller.RemoveInventory(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult);
            Xunit.Assert.Contains("Inventory deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task RemoveInventory_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.RemoveInventory(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            Xunit.Assert.Contains("Invalid inventory ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task RemoveInventory_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.RemoveInventoryAsync(99)).ReturnsAsync(false);

            // Act
            var actionResult = await _controller.RemoveInventory(99);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(actionResult);
            Xunit.Assert.Contains("Inventory with ID 99 not found", notFoundResult.Value.ToString());
        }
    }
}
