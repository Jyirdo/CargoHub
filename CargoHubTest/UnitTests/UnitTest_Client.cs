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
        public async Task GetAllClients_ValidAmount_ReturnsOkResultWithClients()
        {
            // Arrange
            var mockClients = new List<Client>
            {
                new Client { Id = 1, Name = "Client A", ContactEmail = "clienta@test.com" }
            };
            _mockService.Setup(service => service.GetAllClientsAsync(5)).ReturnsAsync(mockClients);

            // Act
            var actionResult = await _controller.GetAllClients(5);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<IEnumerable<Client>>>(actionResult);
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedClients = Xunit.Assert.IsType<List<Client>>(okResult.Value);
            Xunit.Assert.Single(returnedClients);
        }

        [Fact]
        public async Task GetAllClients_InvalidAmount_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetAllClients(0);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<IEnumerable<Client>>>(actionResult);
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid amount", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetClientById_ValidId_ReturnsOkResultWithClient()
        {
            // Arrange
            var mockClient = new Client { Id = 1, Name = "Client A", ContactEmail = "clienta@test.com" };
            _mockService.Setup(service => service.GetClientByIdAsync(1)).ReturnsAsync(mockClient);

            // Act
            var actionResult = await _controller.GetClientById(1);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Client>>(actionResult);
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedClient = Xunit.Assert.IsType<Client>(okResult.Value);
            Xunit.Assert.Equal(mockClient.Id, returnedClient.Id);
        }

        [Fact]
        public async Task GetClientById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetClientById(-1);

            // Assert
            var result = Xunit.Assert.IsType<ActionResult<Client>>(actionResult);
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid client ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task CreateClient_ValidClient_ReturnsCreatedAtAction()
        {
            // Arrange
            var newClient = new Client { Id = 1, Name = "Client A", ContactEmail = "clienta@test.com" };
            _mockService.Setup(service => service.GetClientByEmailAsync(newClient.ContactEmail)).ReturnsAsync((Client)null);
            _mockService.Setup(service => service.CreateClientAsync(newClient)).ReturnsAsync(newClient);

            // Act
            var actionResult = await _controller.CreateClient(newClient);

            // Assert
            var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(actionResult);
            var returnedClient = Xunit.Assert.IsType<Client>(createdAtActionResult.Value);
            Xunit.Assert.Equal(newClient.Id, returnedClient.Id);
        }

        [Fact]
        public async Task CreateClient_InvalidClient_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var actionResult = await _controller.CreateClient(new Client());

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            var errors = badRequestResult.Value as SerializableError;
            Xunit.Assert.NotNull(errors);
            Xunit.Assert.True(errors.ContainsKey("Name"));
        }

        [Fact]
        public async Task UpdateClient_ValidId_ReturnsOkResultWithUpdatedClient()
        {
            // Arrange
            var updatedClient = new Client { Id = 1, Name = "Updated Client", ContactEmail = "updated@test.com" };
            _mockService.Setup(service => service.UpdateClientAsync(updatedClient, 1)).ReturnsAsync(updatedClient);

            // Act
            var actionResult = await _controller.UpdateClient(1, updatedClient);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult);
            var returnedClient = Xunit.Assert.IsType<Client>(okResult.Value);
            Xunit.Assert.Equal(updatedClient.Name, returnedClient.Name);
        }

        [Fact]
        public async Task UpdateClient_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.UpdateClient(-1, new Client());

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            Xunit.Assert.Contains("Invalid client ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task RemoveClientById_ValidId_ReturnsOkResult()
        {
            // Arrange
            _mockService.Setup(service => service.RemoveClientByIdAsync(1)).ReturnsAsync(new Client { Id = 1 });

            // Act
            var actionResult = await _controller.RemoveClientById(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult);
            Xunit.Assert.Contains("Client deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task RemoveClientById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.RemoveClientById(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(actionResult);
            Xunit.Assert.Contains("Invalid client ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task RemoveClientByEmail_ValidEmail_ReturnsOkResult()
        {
            // Arrange
            _mockService.Setup(service => service.RemoveClientByEmailAsync("clienta@test.com")).ReturnsAsync(new Client { Id = 1 });

            // Act
            var actionResult = await _controller.RemoveClientByEmail("clienta@test.com");

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult);
            Xunit.Assert.Contains("Client deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task RemoveClientByEmail_NotFound_ReturnsNoContent()
        {
            // Arrange
            _mockService.Setup(service => service.RemoveClientByEmailAsync("notfound@test.com")).ReturnsAsync((Client)null);

            // Act
            var actionResult = await _controller.RemoveClientByEmail("notfound@test.com");

            // Assert
            Xunit.Assert.IsType<NoContentResult>(actionResult);
        }
    }
}
