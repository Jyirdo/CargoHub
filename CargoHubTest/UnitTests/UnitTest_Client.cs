using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using CargohubV2.Controllers;
using CargohubV2.Models;
using CargohubV2.Services;

namespace CargohubV2.Tests
{
    public class ClientsControllerTests
    {
        private readonly Mock<ClientsService> _mockService;
        private readonly ClientsController _controller;

        public ClientsControllerTests()
        {
            _mockService = new Mock<ClientsService>(null); // Mocked service
            _controller = new ClientsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllClients_ReturnsOkResult_WithListOfClients()
        {
            // Arrange
            var mockClients = new List<Client> { new Client { Id = 1, Name = "Test Client" } };
            _mockService.Setup(service => service.GetAllClientsAsync(10)).ReturnsAsync(mockClients);

            // Act
            var result = await _controller.GetAllClients(10);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedClients = Xunit.Assert.IsType<List<Client>>(okResult.Value);
            Xunit.Assert.Single(returnedClients);
        }

        [Fact]
        public async Task GetClientById_ValidId_ReturnsClient()
        {
            // Arrange
            var mockClient = new Client { Id = 1, Name = "Test Client" };
            _mockService.Setup(service => service.GetClientByIdAsync(1)).ReturnsAsync(mockClient);

            // Act
            var result = await _controller.GetClientById(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedClient = Xunit.Assert.IsType<Client>(okResult.Value);
            Xunit.Assert.Equal(mockClient.Id, returnedClient.Id);
        }

        [Fact]
        public async Task GetClientById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetClientById(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid client ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task CreateClient_ValidClient_ReturnsCreatedAtAction()
        {
            // Arrange
            var newClient = new Client { Id = 1, ContactEmail = "test@example.com" };
            _mockService.Setup(service => service.GetClientByEmailAsync(newClient.ContactEmail)).ReturnsAsync((Client?)null);
            _mockService.Setup(service => service.CreateClientAsync(newClient)).ReturnsAsync(newClient);

            // Act
            var result = await _controller.CreateClient(newClient);

            // Assert
            var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(result);
            var returnedClient = Xunit.Assert.IsType<Client>(createdAtActionResult.Value);
            Xunit.Assert.Equal(newClient.Id, returnedClient.Id);
        }

        [Fact]
        public async Task CreateClient_DuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var existingClient = new Client { Id = 1, ContactEmail = "test@example.com" };
            _mockService.Setup(service => service.GetClientByEmailAsync(existingClient.ContactEmail)).ReturnsAsync(existingClient);

            // Act
            var result = await _controller.CreateClient(existingClient);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            Xunit.Assert.Contains("Client already exists", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task UpdateClient_ValidId_ReturnsUpdatedClient()
        {
            // Arrange
            var updatedClient = new Client { Id = 1, Name = "Updated Name" };
            _mockService.Setup(service => service.UpdateClientAsync(updatedClient, 1)).ReturnsAsync(updatedClient);

            // Act
            var result = await _controller.UpdateClient(1, updatedClient);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnedClient = Xunit.Assert.IsType<Client>(okResult.Value);
            Xunit.Assert.Equal(updatedClient.Name, returnedClient.Name);
        }

        [Fact]
        public async Task RemoveClientById_ValidId_ReturnsSuccessMessage()
        {
            // Arrange
            var mockClient = new Client { Id = 1 };
            _mockService.Setup(service => service.RemoveClientByIdAsync(1)).ReturnsAsync(mockClient);

            // Act
            var result = await _controller.RemoveClientById(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Contains("Client deleted successfully", okResult.Value.ToString());
        }
    }
}
