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
    public class ItemTypesControllerTests
    {
        private readonly Mock<ItemTypeService> _mockService;
        private readonly ItemTypesController _controller;

        public ItemTypesControllerTests()
        {
            _mockService = new Mock<ItemTypeService>(null); // Mocked service
            _controller = new ItemTypesController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllItemTypes_ValidAmount_ReturnsOkResultWithItemTypes()
        {
            // Arrange
            var mockItemTypes = new List<Item_Type>
            {
                new Item_Type { Id = 1, Name = "Type A", Description = "Test Type" }
            };
            _mockService.Setup(service => service.GetAllItemTypesAsync(5)).ReturnsAsync(mockItemTypes);

            // Act
            var actionResult = await _controller.GetAllItemTypes(5);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<IEnumerable<Item_Type>>>(actionResult);
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedItemTypes = Xunit.Assert.IsType<List<Item_Type>>(okResult.Value);
            Xunit.Assert.Single(returnedItemTypes);
        }

        [Fact]
        public async Task GetAllItemTypes_InvalidAmount_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetAllItemTypes(0);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<IEnumerable<Item_Type>>>(actionResult);
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid amount", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetItemTypeById_ValidId_ReturnsOkResultWithItemType()
        {
            // Arrange
            var mockItemType = new Item_Type { Id = 1, Name = "Type A", Description = "Test Type" };
            _mockService.Setup(service => service.GetItemTypeByIdAsync(1)).ReturnsAsync(mockItemType);

            // Act
            var actionResult = await _controller.GetItemTypeById(1);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Type>>(actionResult);
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedItemType = Xunit.Assert.IsType<Item_Type>(okResult.Value);
            Xunit.Assert.Equal(mockItemType.Id, returnedItemType.Id);
        }

        [Fact]
        public async Task GetItemTypeById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetItemTypeById(-1);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Type>>(actionResult);
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid itemtype ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetItemTypeById_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.GetItemTypeByIdAsync(99)).ReturnsAsync((Item_Type)null);

            // Act
            var actionResult = await _controller.GetItemTypeById(99);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Type>>(actionResult);
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
            Xunit.Assert.Contains("Item type with ID 99 not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task AddItemType_ValidItemType_ReturnsCreatedAtAction()
        {
            // Arrange
            var newItemType = new Item_Type { Id = 1, Name = "Type A", Description = "Test Type" };
            _mockService.Setup(service => service.AddItemTypeAsync(newItemType)).ReturnsAsync(newItemType);

            // Act
            var actionResult = await _controller.AddItemType(newItemType);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Type>>(actionResult);
            var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedItemType = Xunit.Assert.IsType<Item_Type>(createdAtActionResult.Value);
            Xunit.Assert.Equal(newItemType.Id, returnedItemType.Id);
        }

        [Fact]
        public async Task AddItemType_InvalidItemType_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var actionResult = await _controller.AddItemType(new Item_Type());

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Type>>(actionResult);
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);

            // Controleer of de validatiefout aanwezig is
            var errors = badRequestResult.Value as SerializableError;
            Xunit.Assert.NotNull(errors);
            Xunit.Assert.True(errors.ContainsKey("Name"));
            var nameErrors = errors["Name"] as string[];
            Xunit.Assert.Contains("The Name field is required.", nameErrors);
        }

        [Fact]
        public async Task UpdateItemType_ValidId_ReturnsOkResultWithUpdatedItemType()
        {
            // Arrange
            var updatedItemType = new Item_Type { Id = 1, Name = "Updated Type", Description = "Updated Description" };
            _mockService.Setup(service => service.UpdateItemTypeAsync(1, updatedItemType)).ReturnsAsync(updatedItemType);

            // Act
            var actionResult = await _controller.UpdateItemType(1, updatedItemType);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult);
            var returnedItemType = Xunit.Assert.IsType<Item_Type>(okResult.Value);
            Xunit.Assert.Equal(updatedItemType.Name, returnedItemType.Name);
        }

        [Fact]
        public async Task UpdateItemType_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.UpdateItemType(-1, new Item_Type());

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            Xunit.Assert.Contains("Invalid itemtype ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteItemType_ValidId_ReturnsOkResultWithSuccessMessage()
        {
            // Arrange
            _mockService.Setup(service => service.DeleteItemTypeAsync(1)).ReturnsAsync(true);

            // Act
            var actionResult = await _controller.DeleteItemType(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult);
            Xunit.Assert.Contains("Item type deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteItemType_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.DeleteItemType(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            Xunit.Assert.Contains("Invalid itemtype ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteItemType_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.DeleteItemTypeAsync(99)).ReturnsAsync(false);

            // Act
            var actionResult = await _controller.DeleteItemType(99);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(actionResult);
            Xunit.Assert.Contains("Item type with ID 99 not found", notFoundResult.Value.ToString());
        }
    }
}