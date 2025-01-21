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
    public class ItemLinesControllerTests
    {
        private readonly Mock<ItemLineService> _mockService;
        private readonly ItemLinesController _controller;

        public ItemLinesControllerTests()
        {
            _mockService = new Mock<ItemLineService>(null); // Mocked service
            _controller = new ItemLinesController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllItemLines_ValidAmount_ReturnsOkResultWithItemLines()
        {
            // Arrange
            var mockItemLines = new List<Item_Line>
            {
                new Item_Line { Id = 1, Name = "Line A", Description = "Test Line" }
            };
            _mockService.Setup(service => service.GetAllItemLinesAsync(5)).ReturnsAsync(mockItemLines);

            // Act
            var actionResult = await _controller.GetAllItemLines(5);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<IEnumerable<Item_Line>>>(actionResult);
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedItemLines = Xunit.Assert.IsType<List<Item_Line>>(okResult.Value);
            Xunit.Assert.Single(returnedItemLines);
        }

        [Fact]
        public async Task GetAllItemLines_InvalidAmount_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetAllItemLines(0);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<IEnumerable<Item_Line>>>(actionResult);
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid amount", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetItemLineById_ValidId_ReturnsOkResultWithItemLine()
        {
            // Arrange
            var mockItemLine = new Item_Line { Id = 1, Name = "Line A", Description = "Test Line" };
            _mockService.Setup(service => service.GetItemLineByIdAsync(1)).ReturnsAsync(mockItemLine);

            // Act
            var actionResult = await _controller.GetItemLineById(1);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Line>>(actionResult);
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedItemLine = Xunit.Assert.IsType<Item_Line>(okResult.Value);
            Xunit.Assert.Equal(mockItemLine.Id, returnedItemLine.Id);
        }

        [Fact]
        public async Task GetItemLineById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetItemLineById(-1);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Line>>(actionResult);
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid itemline ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetItemLineById_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.GetItemLineByIdAsync(99)).ReturnsAsync((Item_Line)null);

            // Act
            var actionResult = await _controller.GetItemLineById(99);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Line>>(actionResult);
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
            Xunit.Assert.Contains("Item line with ID 99 not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task AddItemLine_ValidItemLine_ReturnsCreatedAtAction()
        {
            // Arrange
            var newItemLine = new Item_Line { Id = 1, Name = "Line A", Description = "Test Line" };
            _mockService.Setup(service => service.AddItemLineAsync(newItemLine)).ReturnsAsync(newItemLine);

            // Act
            var actionResult = await _controller.AddItemLine(newItemLine);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Line>>(actionResult);
            var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedItemLine = Xunit.Assert.IsType<Item_Line>(createdAtActionResult.Value);
            Xunit.Assert.Equal(newItemLine.Id, returnedItemLine.Id);
        }

        [Fact]
        public async Task AddItemLine_InvalidItemLine_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var actionResult = await _controller.AddItemLine(new Item_Line());

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Item_Line>>(actionResult);
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);

            var modelStateErrors = (badRequestResult.Value as SerializableError)?.Values;
            Xunit.Assert.NotNull(modelStateErrors);

            var errorMessages = new List<string>();
            foreach (var error in modelStateErrors)
            {
                if (error is string[] errorArray)
                {
                    errorMessages.AddRange(errorArray);
                }
            }
            Xunit.Assert.Contains("The Name field is required.", errorMessages);
        }



        [Fact]
        public async Task UpdateItemLine_ValidId_ReturnsOkResultWithUpdatedItemLine()
        {
            // Arrange
            var updatedItemLine = new Item_Line { Id = 1, Name = "Updated Line", Description = "Updated Description" };
            _mockService.Setup(service => service.UpdateItemLineAsync(1, updatedItemLine)).ReturnsAsync(updatedItemLine);

            // Act
            var actionResult = await _controller.UpdateItemLine(1, updatedItemLine);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult);
            var returnedItemLine = Xunit.Assert.IsType<Item_Line>(okResult.Value);
            Xunit.Assert.Equal(updatedItemLine.Name, returnedItemLine.Name);
        }

        [Fact]
        public async Task UpdateItemLine_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.UpdateItemLine(-1, new Item_Line());

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            Xunit.Assert.Contains("Invalid itemline ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteItemLine_ValidId_ReturnsOkResultWithSuccessMessage()
        {
            // Arrange
            _mockService.Setup(service => service.DeleteItemLineAsync(1)).ReturnsAsync(true);

            // Act
            var actionResult = await _controller.DeleteItemLine(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult);
            Xunit.Assert.Contains("Item line deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteItemLine_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.DeleteItemLine(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            Xunit.Assert.Contains("Invalid itemline ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteItemLine_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.DeleteItemLineAsync(99)).ReturnsAsync(false);

            // Act
            var actionResult = await _controller.DeleteItemLine(99);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(actionResult);
            Xunit.Assert.Contains("Item line with ID 99 not found", notFoundResult.Value.ToString());
        }
    }
}