using CargohubV2.Controllers;
using CargohubV2.Models;
using CargohubV2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoHubTest
{
    [TestClass]
    public class ShipmentsControllerTests
    {
        private Mock<ShipmentService> _mockShipmentService;
        private ShipmentsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockShipmentService = new Mock<ShipmentService>(null); // Pass null for DbContext
            _controller = new ShipmentsController(_mockShipmentService.Object);
        }

        // Test GetAllShipments
        [TestMethod]
        public async Task GetAllShipments_ReturnsOk_WithShipments()
        {
            var shipments = new List<Shipment>
            {
                new Shipment { Id = 1, ShipmentType = "Air" },
                new Shipment { Id = 2, ShipmentType = "Sea" }
            };
            _mockShipmentService.Setup(s => s.GetAllShipmentsAsync()).ReturnsAsync(shipments);

            var result = await _controller.GetAllShipments();

            var okResult = Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult)) as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedShipments = okResult.Value as List<Shipment>;
            Assert.AreEqual(2, returnedShipments.Count);
        }

        [TestMethod]
        public async Task GetAllShipments_ReturnsOk_WithEmptyList()
        {
            var shipments = new List<Shipment>();
            _mockShipmentService.Setup(s => s.GetAllShipmentsAsync()).ReturnsAsync(shipments);

            var result = await _controller.GetAllShipments();

            var okResult = Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult)) as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedShipments = okResult.Value as List<Shipment>;
            Assert.AreEqual(0, returnedShipments.Count);
        }

        // Test GetShipmentById
        [TestMethod]
        public async Task GetShipmentById_ReturnsOk_WhenShipmentExists()
        {
            var shipmentId = 1;
            var shipment = new Shipment { Id = shipmentId, ShipmentType = "Air" };
            _mockShipmentService.Setup(s => s.GetShipmentByIdAsync(shipmentId)).ReturnsAsync(shipment);

            var result = await _controller.GetShipmentById(shipmentId);

            var okResult = Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult)) as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedShipment = okResult.Value as Shipment;
            Assert.AreEqual(shipmentId, returnedShipment.Id);
        }

        [TestMethod]
        public async Task GetShipmentById_ReturnsBadRequest_WhenShipmentIdIsInvalid()
        {
            var invalidShipmentId = -1;

            var result = await _controller.GetShipmentById(invalidShipmentId);

            var badRequestResult = Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult)) as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Invalid shipment ID. It must be a positive integer.", badRequestResult.Value.ToString());
        }

        [TestMethod]
        public async Task GetShipmentById_ReturnsNotFound_WhenShipmentDoesNotExist()
        {
            var shipmentId = 1;
            _mockShipmentService.Setup(s => s.GetShipmentByIdAsync(shipmentId)).ReturnsAsync((Shipment)null);

            var result = await _controller.GetShipmentById(shipmentId);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        // Test AddShipment
        [TestMethod]
        public async Task AddShipment_ReturnsCreatedAtAction_WhenShipmentIsAdded()
        {
            var newShipment = new Shipment { Id = 1, ShipmentType = "Air" };
            _mockShipmentService.Setup(s => s.GetShipmentByIdAsync(newShipment.Id)).ReturnsAsync((Shipment)null);
            _mockShipmentService.Setup(s => s.AddShipmentAsync(newShipment)).ReturnsAsync(newShipment);

            var result = await _controller.AddShipment(newShipment);

            var createdResult = Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult)) as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            var returnedShipment = createdResult.Value as Shipment;
            Assert.AreEqual(newShipment.Id, returnedShipment.Id);
        }

        [TestMethod]
        public async Task AddShipment_ReturnsBadRequest_WhenShipmentAlreadyExists()
        {
            var newShipment = new Shipment { Id = 1, ShipmentType = "Air" };
            _mockShipmentService.Setup(s => s.GetShipmentByIdAsync(newShipment.Id)).ReturnsAsync(newShipment);

            var result = await _controller.AddShipment(newShipment);

            var badRequestResult = Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult)) as BadRequestObjectResult;
            Assert.AreEqual("Shipment already exists", badRequestResult.Value);
        }

        // Test RemoveShipment
        [TestMethod]
        public async Task RemoveShipment_ReturnsOk_WhenShipmentIsRemoved()
        {
            var shipmentId = 1;
            _mockShipmentService.Setup(s => s.RemoveShipmentAsync(shipmentId)).ReturnsAsync(true);

            var result = await _controller.RemoveShipment(shipmentId);

            var okResult = Assert.IsInstanceOfType(result, typeof(OkResult));
            Assert.IsNotNull(okResult);
        }

        [TestMethod]
        public async Task RemoveShipment_ReturnsNoContent_WhenShipmentDoesNotExist()
        {
            var shipmentId = 1;
            _mockShipmentService.Setup(s => s.RemoveShipmentAsync(shipmentId)).ReturnsAsync(false);

            var result = await _controller.RemoveShipment(shipmentId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
