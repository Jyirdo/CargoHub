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
    public class StockTests
    {
        private readonly Mock<OrderService> _mockOrderService;
        private readonly Mock<ShipmentService> _mockShipmentService;
        private readonly Mock<TransferService> _mockTransferService;
        private readonly OrdersController _ordersController;
        private readonly ShipmentsController _shipmentsController;
        private readonly TransfersController _transfersController;

        public StockTests()
        {
            _mockOrderService = new Mock<OrderService>(null); // Mocked OrderService
            _mockShipmentService = new Mock<ShipmentService>(null); // Mocked ShipmentService
            _mockTransferService = new Mock<TransferService>(null); // Mocked TransferService

            _ordersController = new OrdersController(_mockOrderService.Object);
            _shipmentsController = new ShipmentsController(_mockShipmentService.Object);
            _transfersController = new TransfersController(_mockTransferService.Object);
        }

        [Fact]
        public async Task GetItemsInOrder_ValidOrderId_ReturnsOrderStocks()
        {
            // Arrange
            var mockStocks = new List<OrderStock>
            {
                new OrderStock { Id = 1, ItemId = "Item1", Quantity = 10 }
            };
            _mockOrderService.Setup(service => service.GetItemsInOrderAsync(1)).ReturnsAsync(mockStocks);

            // Act
            var result = await _ordersController.GetItemsInOrder(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedStocks = Xunit.Assert.IsType<List<OrderStock>>(okResult.Value);
            Xunit.Assert.Single(returnedStocks);
        }

        [Fact]
        public async Task GetItemsInOrder_InvalidOrderId_ReturnsBadRequest()
        {
            // Act
            var result = await _ordersController.GetItemsInOrder(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid order ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetItemsInShipment_ValidShipmentId_ReturnsShipmentStocks()
        {
            // Arrange
            var mockStocks = new List<ShipmentStock>
            {
                new ShipmentStock { Id = 1, ItemId = "Item1", Quantity = 15 }
            };
            _mockShipmentService.Setup(service => service.GetItemsInShipmentAsync(1)).ReturnsAsync(mockStocks);

            // Act
            var result = await _shipmentsController.GetItemsInShipment(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedStocks = Xunit.Assert.IsType<List<ShipmentStock>>(okResult.Value);
            Xunit.Assert.Single(returnedStocks);
        }

        [Fact]
        public async Task GetItemsInShipment_InvalidShipmentId_ReturnsBadRequest()
        {
            // Act
            var result = await _shipmentsController.GetItemsInShipment(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid shipment ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task UpdateItemsInShipment_ValidShipmentId_ReturnsOkResult()
        {
            // Arrange
            var updatedItems = new List<ShipmentStock>
            {
                new ShipmentStock { Id = 1, ItemId = "Item1", Quantity = 20 }
            };
            _mockShipmentService.Setup(service => service.UpdateItemsInShipmentAsync(1, updatedItems)).ReturnsAsync(true);

            // Act
            var result = await _shipmentsController.UpdateItemsInShipment(1, updatedItems);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Equal(true, okResult.Value);
        }

        [Fact]
        public async Task UpdateItemsInShipment_InvalidShipmentId_ReturnsBadRequest()
        {
            // Arrange
            var updatedItems = new List<ShipmentStock>
            {
                new ShipmentStock { Id = 1, ItemId = "Item1", Quantity = 20 }
            };

            // Act
            var result = await _shipmentsController.UpdateItemsInShipment(-1, updatedItems);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            Xunit.Assert.Contains("Invalid shipment ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetTransferById_ValidId_ReturnsTransferWithStocks()
        {
            // Arrange
            var mockTransfer = new Transfer
            {
                Id = 1,
                TransferStatus = "Completed",
                Stocks = new List<TransferStock>
                {
                    new TransferStock { Id = 1, ItemId = "Item1", Quantity = 25 }
                }
            };
            _mockTransferService.Setup(service => service.GetTransferByIdAsync(1)).ReturnsAsync(mockTransfer);

            // Act
            var result = await _transfersController.GetTransferById(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnedTransfer = Xunit.Assert.IsType<Transfer>(okResult.Value);
            Xunit.Assert.Single(returnedTransfer.Stocks);
        }

        [Fact]
        public async Task GetTransferById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _transfersController.GetTransferById(-1);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
            Xunit.Assert.Contains("Invalid transfer ID", badRequestResult.Value.ToString());
        }
    }
}
