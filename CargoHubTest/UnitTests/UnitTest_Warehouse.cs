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
    public class WarehousesControllerTests
    {
        private readonly Mock<WarehouseService> _mockService;
        private readonly WarehousesController _controller;

        public WarehousesControllerTests()
        {
            _mockService = new Mock<WarehouseService>(null); // Mocked service
            _controller = new WarehousesController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllWarehouses_ValidAmount_ReturnsOkResultWithWarehouses()
        {
            // Arrange
            var mockWarehouses = new List<Warehouse> 
            { 
                new Warehouse { Id = 1, Name = "Warehouse A" } 
            };
            _mockService.Setup(service => service.GetAllWarehousesAsync(5)).ReturnsAsync(mockWarehouses);

            // Act
            var result = await _controller.GetAllWarehouses(5);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedWarehouses = Xunit.Assert.IsType<List<Warehouse>>(okResult.Value);
            Xunit.Assert.Single(returnedWarehouses);
        }

        [Fact]
        public async Task GetWarehouseById_ValidId_ReturnsOkResultWithWarehouse()
        {
            // Arrange
            var mockWarehouse = new Warehouse { Id = 1, Name = "Warehouse A" };
            _mockService.Setup(service => service.GetWarehouseByIdAsync(1)).ReturnsAsync(mockWarehouse);

            // Act
            var result = await _controller.GetWarehouseById(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedWarehouse = Xunit.Assert.IsType<Warehouse>(okResult.Value);
            Xunit.Assert.Equal(mockWarehouse.Id, returnedWarehouse.Id);
        }

        [Fact]
        public async Task GetWarehouseById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetWarehouseById(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid warehouse ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetWarehousesByCity_ValidCity_ReturnsOkResultWithWarehouses()
        {
            // Arrange
            var mockWarehouses = new List<Warehouse> 
            { 
                new Warehouse { Id = 1, City = "Amsterdam" } 
            };
            _mockService.Setup(service => service.GetWarehousesByCityAsync("Amsterdam")).ReturnsAsync(mockWarehouses);

            // Act
            var result = await _controller.GetWarehousesByCity("Amsterdam");

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedWarehouses = Xunit.Assert.IsType<List<Warehouse>>(okResult.Value);
            Xunit.Assert.Single(returnedWarehouses);
        }

        [Fact]
        public async Task AddWarehouse_ValidWarehouse_ReturnsCreatedAtAction()
        {
            // Arrange
            var newWarehouse = new Warehouse { Id = 1, Name = "Warehouse A" };
            _mockService.Setup(service => service.AddWarehouseAsync(newWarehouse)).ReturnsAsync(newWarehouse);

            // Act
            var result = await _controller.AddWarehouse(newWarehouse);

            // Assert
            var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedWarehouse = Xunit.Assert.IsType<Warehouse>(createdAtActionResult.Value);
            Xunit.Assert.Equal(newWarehouse.Id, returnedWarehouse.Id);
        }

        [Fact]
        public async Task UpdateWarehouse_ValidId_ReturnsOkResultWithUpdatedWarehouse()
        {
            // Arrange
            var updatedWarehouse = new Warehouse { Id = 1, Name = "Updated Warehouse" };
            _mockService.Setup(service => service.UpdateWarehouseAsync(1, updatedWarehouse)).ReturnsAsync(updatedWarehouse);

            // Act
            var result = await _controller.UpdateWarehouse(1, updatedWarehouse);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnedWarehouse = Xunit.Assert.IsType<Warehouse>(okResult.Value);
            Xunit.Assert.Equal(updatedWarehouse.Name, returnedWarehouse.Name);
        }

        [Fact]
        public async Task UpdateWarehouse_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UpdateWarehouse(-1, new Warehouse());

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            Xunit.Assert.Contains("Invalid warehouse ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteWarehouseById_ValidId_ReturnsOkResultWithSuccessMessage()
        {
            // Arrange
            _mockService.Setup(service => service.DeleteWarehouseByIdAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteWarehouseById(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Contains("Warehouse deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteWarehouseById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.DeleteWarehouseById(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            Xunit.Assert.Contains("Invalid warehouse ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteWarehouseById_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.DeleteWarehouseByIdAsync(99)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteWarehouseById(99);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result);
            Xunit.Assert.Contains("Warehouse with ID 99 not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task GetAllWarehouses_ZeroAmount_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetAllWarehouses(0);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid amount", badRequestResult.Value.ToString());
        }
        
        [Fact]
        public async Task UpdateWarehouse_NotFound_ReturnsNotFound()
        {
            // Arrange
            var mockWarehouse = new Warehouse { Id = 1, Name = "Warehouse A" };
            _mockService.Setup(service => service.UpdateWarehouseAsync(99, mockWarehouse)).ReturnsAsync((Warehouse)null);

            // Act
            var result = await _controller.UpdateWarehouse(99, mockWarehouse);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result);
            Xunit.Assert.Contains("Warehouse with ID 99 not found", notFoundResult.Value.ToString());
        }
    }
}
