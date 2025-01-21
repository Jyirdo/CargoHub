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
    public class LocationControllerTests
    {
        private readonly Mock<LocationService> _mockService;
        private readonly LocationController _controller;

        public LocationControllerTests()
        {
            _mockService = new Mock<LocationService>(null); // Mocked service
            _controller = new LocationController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllLocations_ValidAmount_ReturnsOkResultWithLocations()
        {
            // Arrange
            var mockLocations = new List<Location>
            {
                new Location { Id = 1, Name = "Location A" }
            };
            _mockService.Setup(service => service.GetAllLocationsAsync(5)).ReturnsAsync(mockLocations);

            // Act
            var result = await _controller.GetAllLocations(5);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedLocations = Xunit.Assert.IsType<List<Location>>(okResult.Value);
            Xunit.Assert.Single(returnedLocations);
        }

        [Fact]
        public async Task GetAllLocations_InvalidAmount_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetAllLocations(0);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid amount", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetLocationById_ValidId_ReturnsOkResultWithLocation()
        {
            // Arrange
            var mockLocation = new Location { Id = 1, Name = "Location A" };
            _mockService.Setup(service => service.GetLocationByIdAsync(1)).ReturnsAsync(mockLocation);

            // Act
            var result = await _controller.GetLocationById(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedLocation = Xunit.Assert.IsType<Location>(okResult.Value);
            Xunit.Assert.Equal(mockLocation.Id, returnedLocation.Id);
        }

        [Fact]
        public async Task GetLocationById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetLocationById(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid location ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetLocationById_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.GetLocationByIdAsync(99)).ReturnsAsync((Location)null);

            // Act
            var result = await _controller.GetLocationById(99);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
            Xunit.Assert.Contains("Location with ID 99 not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task SearchByName_ValidName_ReturnsOkResultWithLocations()
        {
            // Arrange
            var mockLocations = new List<Location>
            {
                new Location { Id = 1, Name = "Location A" }
            };
            _mockService.Setup(service => service.SearchLocationsByNameAsync("Location A")).ReturnsAsync(mockLocations);

            // Act
            var result = await _controller.SearchByName("Location A");

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedLocations = Xunit.Assert.IsType<List<Location>>(okResult.Value);
            Xunit.Assert.Single(returnedLocations);
        }

        [Fact]
        public async Task SearchByCode_ValidCode_ReturnsOkResultWithLocation()
        {
            // Arrange
            var mockLocation = new Location { Id = 1, Code = "LOC123" };
            _mockService.Setup(service => service.SearchLocationByCodeAsync("LOC123")).ReturnsAsync(mockLocation);

            // Act
            var result = await _controller.SearchByCode("LOC123");

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedLocation = Xunit.Assert.IsType<Location>(okResult.Value);
            Xunit.Assert.Equal(mockLocation.Code, returnedLocation.Code);
        }

        [Fact]
        public async Task SearchByCode_InvalidCode_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.SearchLocationByCodeAsync("INVALID")).ReturnsAsync((Location)null);

            // Act
            var result = await _controller.SearchByCode("INVALID");

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
            Xunit.Assert.Contains("Location with Code 'INVALID' not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task AddLocation_ValidLocation_ReturnsCreatedAtAction()
        {
            // Arrange
            var newLocation = new Location { Id = 1, Name = "New Location" };
            _mockService.Setup(service => service.AddLocationAsync(newLocation)).ReturnsAsync(newLocation);

            // Act
            var result = await _controller.AddLocation(newLocation);

            // Assert
            var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedLocation = Xunit.Assert.IsType<Location>(createdAtActionResult.Value);
            Xunit.Assert.Equal(newLocation.Id, returnedLocation.Id);
        }

        [Fact]
        public async Task UpdateLocation_ValidId_ReturnsOkResultWithUpdatedLocation()
        {
            // Arrange
            var updatedLocation = new Location { Id = 1, Name = "Updated Location" };
            _mockService.Setup(service => service.UpdateLocationAsync(1, updatedLocation)).ReturnsAsync(updatedLocation);

            // Act
            var result = await _controller.UpdateLocation(1, updatedLocation);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnedLocation = Xunit.Assert.IsType<Location>(okResult.Value);
            Xunit.Assert.Equal(updatedLocation.Name, returnedLocation.Name);
        }

        [Fact]
        public async Task UpdateLocation_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UpdateLocation(-1, new Location());

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            Xunit.Assert.Contains("Invalid location ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteLocationById_ValidId_ReturnsOkResult()
        {
            // Arrange
            _mockService.Setup(service => service.DeleteLocationByIdAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteLocationById(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Contains("Location deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task GetLocationCount_ReturnsOkResultWithCount()
        {
            // Arrange
            _mockService.Setup(service => service.GetLocationCountAsync()).ReturnsAsync(10);

            // Act
            var result = await _controller.GetLocationCount();

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var count = Xunit.Assert.IsType<int>(okResult.Value);
            Xunit.Assert.Equal(10, count);
        }
    }
}
