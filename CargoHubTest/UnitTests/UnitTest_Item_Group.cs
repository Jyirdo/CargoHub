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
    public class ItemGroupsControllerTests
    {
        private readonly Mock<ItemGroupService> _mockService;
        private readonly ItemGroupsController _controller;

        public ItemGroupsControllerTests()
        {
            _mockService = new Mock<ItemGroupService>(null); // Mocked service
            _controller = new ItemGroupsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllItemGroups_ValidAmount_ReturnsOkResultWithItemGroups()
        {
            // Arrange
            var mockItemGroups = new List<Item_Group>
            {
                new Item_Group { Id = 1, Name = "Group A", Description = "Test Group" }
            };
            _mockService.Setup(service => service.GetAllItemGroupsAsync(5)).ReturnsAsync(mockItemGroups);

            // Act
            var actionResult = await _controller.GetAllItemGroups(5);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<IEnumerable<Item_Group>>>(actionResult);
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedItemGroups = Xunit.Assert.IsType<List<Item_Group>>(okResult.Value);
            Xunit.Assert.Single(returnedItemGroups);
        }

        [Fact]
        public async Task GetAllItemGroups_InvalidAmount_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetAllItemGroups(0);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<IEnumerable<Item_Group>>>(actionResult);
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid amount", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetItemGroupById_ValidId_ReturnsOkResultWithItemGroup()
        {
            // Arrange
            var mockItemGroup = new Item_Group { Id = 1, Name = "Group A", Description = "Test Group" };
            _mockService.Setup(service => service.GetItemGroupByIdAsync(1)).ReturnsAsync(mockItemGroup);

            // Act
            var actionResult = await _controller.GetItemGroupById(1);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Group>>(actionResult);
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedItemGroup = Xunit.Assert.IsType<Item_Group>(okResult.Value);
            Xunit.Assert.Equal(mockItemGroup.Id, returnedItemGroup.Id);
        }

        [Fact]
        public async Task GetItemGroupById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetItemGroupById(-1);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Group>>(actionResult);
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid itemgroup ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetItemGroupById_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.GetItemGroupByIdAsync(99)).ReturnsAsync((Item_Group)null);

            // Act
            var actionResult = await _controller.GetItemGroupById(99);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Group>>(actionResult);
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
            Xunit.Assert.Contains("Item group with ID 99 not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task AddItemGroup_ValidItemGroup_ReturnsCreatedAtAction()
        {
            // Arrange
            var newItemGroup = new Item_Group { Id = 1, Name = "Group A", Description = "Test Group" };
            _mockService.Setup(service => service.AddItemGroupAsync(newItemGroup)).ReturnsAsync(newItemGroup);

            // Act
            var actionResult = await _controller.AddItemGroup(newItemGroup);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Group>>(actionResult);
            var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedItemGroup = Xunit.Assert.IsType<Item_Group>(createdAtActionResult.Value);
            Xunit.Assert.Equal(newItemGroup.Id, returnedItemGroup.Id);
        }

        [Fact]
        public async Task AddItemGroup_InvalidItemGroup_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var actionResult = await _controller.AddItemGroup(new Item_Group());

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Group>>(actionResult);
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);

            // Controleer of de fout aanwezig is in het ModelState-object
            var errors = badRequestResult.Value as SerializableError;
            Xunit.Assert.NotNull(errors);
            Xunit.Assert.True(errors.ContainsKey("Name"));
            Xunit.Assert.Contains("The Name field is required.", errors["Name"] as string[]);
        }

        [Fact]
        public async Task UpdateItemGroup_ValidId_ReturnsOkResultWithUpdatedItemGroup()
        {
            // Arrange
            var updatedItemGroup = new Item_Group { Id = 1, Name = "Updated Group", Description = "Updated Description" };
            _mockService.Setup(service => service.UpdateItemGroupAsync(1, updatedItemGroup)).ReturnsAsync(updatedItemGroup);

            // Act
            var actionResult = await _controller.UpdateItemGroup(1, updatedItemGroup);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult);
            var returnedItemGroup = Xunit.Assert.IsType<Item_Group>(okResult.Value);
            Xunit.Assert.Equal(updatedItemGroup.Name, returnedItemGroup.Name);
        }

        [Fact]
        public async Task UpdateItemGroup_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.UpdateItemGroup(-1, new Item_Group());

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            Xunit.Assert.Contains("Invalid itemgroup ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteItemGroup_ValidId_ReturnsOkResultWithSuccessMessage()
        {
            // Arrange
            _mockService.Setup(service => service.RemoveItemGroupAsync(1)).ReturnsAsync(true);

            // Act
            var actionResult = await _controller.DeleteItemGroup(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult);
            Xunit.Assert.Contains("Item group deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteItemGroup_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.DeleteItemGroup(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            Xunit.Assert.Contains("Invalid itemgroup ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteItemGroup_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.RemoveItemGroupAsync(99)).ReturnsAsync(false);

            // Act
            var actionResult = await _controller.DeleteItemGroup(99);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(actionResult);
            Xunit.Assert.Contains("Item group with ID 99 not found", notFoundResult.Value.ToString());
        }
    }
}