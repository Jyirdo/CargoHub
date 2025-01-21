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
    public class SupplierControllerTests
    {
        private readonly Mock<SupplierService> _mockService;
        private readonly SupplierController _controller;

        public SupplierControllerTests()
        {
            _mockService = new Mock<SupplierService>(null); // Mocked service
            _controller = new SupplierController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllSuppliers_ValidAmount_ReturnsOkResultWithSuppliers()
        {
            // Arrange
            var mockSuppliers = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "Supplier A" }
            };
            _mockService.Setup(service => service.GetAllSuppliersAsync(5)).ReturnsAsync(mockSuppliers);

            // Act
            var result = await _controller.GetAllSuppliers(5);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedSuppliers = Xunit.Assert.IsType<List<Supplier>>(okResult.Value);
            Xunit.Assert.Single(returnedSuppliers);
        }

        [Fact]
        public async Task GetAllSuppliers_InvalidAmount_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetAllSuppliers(0);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid amount", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetSupplierById_ValidId_ReturnsOkResultWithSupplier()
        {
            // Arrange
            var mockSupplier = new Supplier { Id = 1, Name = "Supplier A" };
            _mockService.Setup(service => service.GetSupplierByIdAsync(1)).ReturnsAsync(mockSupplier);

            // Act
            var result = await _controller.GetSupplierById(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedSupplier = Xunit.Assert.IsType<Supplier>(okResult.Value);
            Xunit.Assert.Equal(mockSupplier.Id, returnedSupplier.Id);
        }

        [Fact]
        public async Task GetSupplierById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetSupplierById(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid supplier ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetSupplierById_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.GetSupplierByIdAsync(99)).ReturnsAsync((Supplier)null);

            // Act
            var result = await _controller.GetSupplierById(99);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
            Xunit.Assert.Contains("Supplier with ID 99 not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task SearchByName_ValidName_ReturnsOkResultWithSuppliers()
        {
            // Arrange
            var mockSuppliers = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "Supplier A" }
            };
            _mockService.Setup(service => service.SearchSuppliersByNameAsync("Supplier A")).ReturnsAsync(mockSuppliers);

            // Act
            var result = await _controller.SearchByName("Supplier A");

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedSuppliers = Xunit.Assert.IsType<List<Supplier>>(okResult.Value);
            Xunit.Assert.Single(returnedSuppliers);
        }

        [Fact]
        public async Task SearchByCity_ValidCity_ReturnsOkResultWithSuppliers()
        {
            // Arrange
            var mockSuppliers = new List<Supplier>
            {
                new Supplier { Id = 1, City = "Amsterdam" }
            };
            _mockService.Setup(service => service.SearchSuppliersByCityAsync("Amsterdam")).ReturnsAsync(mockSuppliers);

            // Act
            var result = await _controller.SearchByCity("Amsterdam");

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedSuppliers = Xunit.Assert.IsType<List<Supplier>>(okResult.Value);
            Xunit.Assert.Single(returnedSuppliers);
        }

        [Fact]
        public async Task GetSuppliersByDateRange_ValidRange_ReturnsOkResultWithSuppliers()
        {
            // Arrange
            var mockSuppliers = new List<Supplier>
            {
                new Supplier { Id = 1, CreatedAt = DateTime.UtcNow.AddDays(-1) }
            };
            _mockService.Setup(service => service.GetSuppliersByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(mockSuppliers);

            // Act
            var result = await _controller.GetSuppliersByDateRange(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedSuppliers = Xunit.Assert.IsType<List<Supplier>>(okResult.Value);
            Xunit.Assert.Single(returnedSuppliers);
        }

        [Fact]
        public async Task CheckDuplicate_ValidSupplier_ReturnsOkResultWithTrue()
        {
            // Arrange
            var mockSupplier = new Supplier { Name = "Supplier A", City = "Amsterdam", Country = "Netherlands" };
            _mockService.Setup(service => service.CheckDuplicateSupplierAsync(mockSupplier)).ReturnsAsync(true);

            // Act
            var result = await _controller.CheckDuplicate(mockSupplier);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var isDuplicate = Xunit.Assert.IsType<bool>(okResult.Value);
            Xunit.Assert.True(isDuplicate);
        }

        [Fact]
        public async Task DeleteBatch_ValidIds_ReturnsOkResultWithSuccessMessage()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            _mockService.Setup(service => service.DeleteSuppliersBatchAsync(ids)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteBatch(ids);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Contains("Suppliers deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteBatch_InvalidIds_ReturnsNotFound()
        {
            // Arrange
            var ids = new List<int> { 99, 100 };
            _mockService.Setup(service => service.DeleteSuppliersBatchAsync(ids)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBatch(ids);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result);
            Xunit.Assert.Contains("Some suppliers were not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task GetSupplierCount_ReturnsOkResultWithCount()
        {
            // Arrange
            _mockService.Setup(service => service.GetSupplierCountAsync()).ReturnsAsync(10);

            // Act
            var result = await _controller.GetSupplierCount();

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var count = Xunit.Assert.IsType<int>(okResult.Value);
            Xunit.Assert.Equal(10, count);
        }

        [Fact]
        public async Task CreateSupplier_ValidSupplier_ReturnsCreatedAtAction()
        {
            // Arrange
            var newSupplier = new Supplier { Id = 1, Name = "Supplier A" };
            _mockService.Setup(service => service.AddSupplierAsync(newSupplier)).ReturnsAsync(newSupplier);

            // Act
            var result = await _controller.CreateSupplier(newSupplier);

            // Assert
            var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedSupplier = Xunit.Assert.IsType<Supplier>(createdAtActionResult.Value);
            Xunit.Assert.Equal(newSupplier.Id, returnedSupplier.Id);
        }

        [Fact]
        public async Task CreateSupplier_NullSupplier_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.CreateSupplier(null);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Supplier data is required", badRequestResult.Value.ToString());
        }
    }
}
