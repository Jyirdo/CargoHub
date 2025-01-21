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
    public class TransfersControllerTests
    {
        private readonly Mock<TransferService> _mockService;
        private readonly TransfersController _controller;

        public TransfersControllerTests()
        {
            _mockService = new Mock<TransferService>(null); // Mocked service
            _controller = new TransfersController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllTransfers_ValidAmount_ReturnsOkResultWithTransfers()
        {
            // Arrange
            var mockTransfers = new List<Transfer> { new Transfer { Id = 1, Reference = "TR123" } };
            _mockService.Setup(service => service.GetAllTransfersAsync(5)).ReturnsAsync(mockTransfers);

            // Act
            var result = await _controller.GetAllTransfers(5);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedTransfers = Xunit.Assert.IsType<List<Transfer>>(okResult.Value);
            Xunit.Assert.Single(returnedTransfers);
        }

        [Fact]
        public async Task GetAllTransfers_InvalidAmount_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetAllTransfers(0);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid amount", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetTransferById_ValidId_ReturnsOkResultWithTransfer()
        {
            // Arrange
            var mockTransfer = new Transfer { Id = 1, Reference = "TR123" };
            _mockService.Setup(service => service.GetTransferByIdAsync(1)).ReturnsAsync(mockTransfer);

            // Act
            var result = await _controller.GetTransferById(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedTransfer = Xunit.Assert.IsType<Transfer>(okResult.Value);
            Xunit.Assert.Equal(mockTransfer.Id, returnedTransfer.Id);
        }

        [Fact]
        public async Task GetTransferById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetTransferById(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid transfer ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetTransferById_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.GetTransferByIdAsync(99)).ReturnsAsync((Transfer)null);

            // Act
            var result = await _controller.GetTransferById(99);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
            Xunit.Assert.Contains("Transfer with ID 99 not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task GetTransfersByStatus_ValidStatus_ReturnsOkResultWithTransfers()
        {
            // Arrange
            var mockTransfers = new List<Transfer> { new Transfer { Id = 1, TransferStatus = "Completed" } };
            _mockService.Setup(service => service.GetTransfersByStatusAsync("Completed")).ReturnsAsync(mockTransfers);

            // Act
            var result = await _controller.GetTransfersByStatus("Completed");

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedTransfers = Xunit.Assert.IsType<List<Transfer>>(okResult.Value);
            Xunit.Assert.Single(returnedTransfers);
        }

        [Fact]
        public async Task AddTransfer_ValidTransfer_ReturnsCreatedAtAction()
        {
            // Arrange
            var newTransfer = new Transfer { Id = 1, Reference = "TR123" };
            _mockService.Setup(service => service.AddTransferAsync(newTransfer)).ReturnsAsync(newTransfer);

            // Act
            var result = await _controller.AddTransfer(newTransfer);

            // Assert
            var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedTransfer = Xunit.Assert.IsType<Transfer>(createdAtActionResult.Value);
            Xunit.Assert.Equal(newTransfer.Id, returnedTransfer.Id);
        }

        [Fact]
        public async Task UpdateTransfer_ValidId_ReturnsOkResultWithUpdatedTransfer()
        {
            // Arrange
            var updatedTransfer = new Transfer { Id = 1, Reference = "TR124" };
            _mockService.Setup(service => service.UpdateTransferAsync(1, updatedTransfer)).ReturnsAsync(updatedTransfer);

            // Act
            var result = await _controller.UpdateTransfer(1, updatedTransfer);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnedTransfer = Xunit.Assert.IsType<Transfer>(okResult.Value);
            Xunit.Assert.Equal(updatedTransfer.Reference, returnedTransfer.Reference);
        }

        [Fact]
        public async Task UpdateTransfer_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UpdateTransfer(-1, new Transfer());

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            Xunit.Assert.Contains("Invalid transfer ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteTransferById_ValidId_ReturnsOkResultWithSuccessMessage()
        {
            // Arrange
            _mockService.Setup(service => service.DeleteTransferByIdAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTransferById(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Contains("Transfer deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteTransferById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.DeleteTransferById(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            Xunit.Assert.Contains("Invalid transfer ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteTransferById_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.DeleteTransferByIdAsync(99)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTransferById(99);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result);
            Xunit.Assert.Contains("Transfer with ID 99 not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteTransfersByStatus_ValidStatus_ReturnsNoContent()
        {
            // Arrange
            _mockService.Setup(service => service.DeleteTransfersByStatusAsync("Completed")).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTransfersByStatus("Completed");

            // Assert
            Xunit.Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTransfersByStatus_NoTransfers_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.DeleteTransfersByStatusAsync("NonexistentStatus")).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTransfersByStatus("NonexistentStatus");

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result);
            Xunit.Assert.Contains("No transfers found with status 'NonexistentStatus'.", notFoundResult.Value.ToString());
        }
    }
}
