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
    public class ItemsControllerTests
    {
        private readonly Mock<ItemService> _mockService;
        private readonly ItemsController _controller;

        public ItemsControllerTests()
        {
            _mockService = new Mock<ItemService>(null); // Mocked service
            _controller = new ItemsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllItems_ValidAmount_ReturnsOkResultWithItems()
        {
            // Arrange
            var mockItems = new List<Item>
            {
                new Item { Id = 1, UId = "UID001", Code = "CODE1" }
            };
            _mockService.Setup(service => service.GetAllItemsAsync(5)).ReturnsAsync(mockItems);

            // Act
            var actionResult = await _controller.GetAllItems(5);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedItems = Xunit.Assert.IsType<List<Item>>(okResult.Value);
            Xunit.Assert.Single(returnedItems);
        }

        [Fact]
        public async Task GetAllItems_InvalidAmount_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetAllItems(0);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Xunit.Assert.Contains("Invalid amount", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetItemById_ValidId_ReturnsOkResultWithItem()
        {
            // Arrange
            var mockItem = new Item { Id = 1, UId = "UID001", Code = "CODE1" };
            _mockService.Setup(service => service.GetItemByUidAsync("UID001")).ReturnsAsync(mockItem);

            // Act
            var actionResult = await _controller.GetItemById("UID001");

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedItem = Xunit.Assert.IsType<Item>(okResult.Value);
            Xunit.Assert.Equal(mockItem.UId, returnedItem.UId);
        }

        [Fact]
        public async Task GetItemById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.GetItemByUidAsync("INVALID_UID")).ReturnsAsync((Item)null);

            // Act
            var actionResult = await _controller.GetItemById("INVALID_UID");

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Xunit.Assert.Contains("Item with UID INVALID_UID not found", notFoundResult.Value.ToString());
        }
        
        [Fact]
        public async Task GetItemsByItemLine_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetItemsByItemLine(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Xunit.Assert.Contains("Invalid itemsline ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task UpdateItem_ValidId_ReturnsNoContent()
        {
            // Arrange
            var updatedItem = new Item { Id = 1, UId = "UID002" };
            _mockService.Setup(service => service.UpdateItemAsync(1, updatedItem)).ReturnsAsync(true);

            // Act
            var actionResult = await _controller.UpdateItem(1, updatedItem);

            // Assert
            Xunit.Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task UpdateItem_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var updatedItem = new Item { Id = 2, UId = "UID002" };

            // Act
            var actionResult = await _controller.UpdateItem(1, updatedItem);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            Xunit.Assert.Contains("ID in the URL does not match the ID in the payload", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task RemoveItem_ValidId_ReturnsOkResultWithSuccessMessage()
        {
            // Arrange
            _mockService.Setup(service => service.RemoveItemAsync(1)).ReturnsAsync(true);

            // Act
            var actionResult = await _controller.RemoveItem(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult);
            Xunit.Assert.Contains("Item deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task RemoveItem_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.RemoveItemAsync(99)).ReturnsAsync(false);

            // Act
            var actionResult = await _controller.RemoveItem(99);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(actionResult);
            Xunit.Assert.Contains("Item with ID 99 not found", notFoundResult.Value.ToString());
        }
    }
}
