using CargohubV2;
using CargohubV2.Contexts;
using CargohubV2.Services;
using CargohubV2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace UnitTests
{
    [TestClass]
    public class UnitTest_Client
    {
        private CargoHubDbContext _dbContext;
        private ClientsService _clientService;
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CargoHubDbContext>()
                .UseSqlite("DataSource=:memory:")  // Use SQLite in-memory database
                .Options;

            _dbContext = new CargoHubDbContext(options);
            _clientService = new ClientsService(_dbContext);

            // Open the connection so it can be used
            _dbContext.Database.OpenConnection();
            _dbContext.Database.EnsureCreated();  // Ensure the database is created

            SeedDatabase(_dbContext);  // Seed the database with initial data
        }


        private void SeedDatabase(CargoHubDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Clients.AddRange(
                new Client
                {
                    Name = "Vincent",
                    Address = "123 Street A",
                    City = "Oosterland",
                    ZipCode = "12345",
                    Province = "Zeeland",
                    Country = "Netherlands",
                    ContactName = "Contact A",
                    ContactPhone = "1234567890",
                    ContactEmail = "vincent@gmail.com",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Client
                {
                    Name = "Xander",
                    Address = "456 Street B",
                    City = "CityB",
                    ZipCode = "67890",
                    Province = "Kiev",
                    Country = "Ukraine",
                    ContactName = "Xandertje",
                    ContactPhone = "011231231",
                    ContactEmail = "xanderbos@gmail.com",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Client
                {
                    Name = "Nolan",
                    Address = "456 Street B",
                    City = "CityB",
                    ZipCode = "67890",
                    Province = "Groningen",
                    Country = "Netherlands",
                    ContactName = "Contact B",
                    ContactPhone = "012312312",
                    ContactEmail = "nolananimations@gmail.com",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Client
                {
                    Name = "Efraim",
                    Address = "456 Street B",
                    City = "Ridderkerk",
                    ZipCode = "67890",
                    Province = "Zuid-Holland",
                    Country = "Netherlands",
                    ContactName = "Efraimpie",
                    ContactPhone = "031231231",
                    ContactEmail = "efraimcreampie@gmail.com",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            context.SaveChanges();
        }

        [TestMethod]
        public async Task TestGetAllClients()
        {
            var clients = await _clientService.GetAllClientsAsync(10);
            Assert.AreEqual(4, clients.Count);
        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(999, false)]
        public async Task TestGetClientById(int clientId, bool exists)
        {
            var client = await _clientService.GetClientByIdAsync(clientId);
            Assert.AreEqual(exists, client != null);
        }

        [TestMethod]
        public async Task TestAddClient()
        {
            var client = new Client
            {
                Name = "Raymond Inc",
                Address = "1296 Daniel Road Apt. 349",
                City = "Pierceview",
                ZipCode = "28301",
                Province = "Colorado",
                Country = "United States",
                ContactName = "Bryan Clark",
                ContactPhone = "242.732.3483x2573",
                ContactEmail = "robertcharles@example.net"
            };

            var newClient = await _clientService.CreateClientAsync(client);

            Assert.IsNotNull(newClient);
            Assert.AreEqual(5, (await _clientService.GetAllClientsAsync(10)).Count);
        }

        [TestMethod]
        [DataRow("vincent@gmail.com", false)]
        [DataRow("newclient@example.com", true)]
        [DataRow("xanderbos@gmail.com", false)]
        [DataRow("uniqueemail123@example.com", true)]
        public async Task TestClientEmailDuplicateCheck(string email, bool expectedResult)
        {
            var isEmailUnique = await _clientService.GetClientByEmailAsync(email) == null;
            Assert.AreEqual(expectedResult, isEmailUnique);
        }

        [TestMethod]
        [DataRow(1, "Updated Name", true)]
        [DataRow(999, "Nonexistent", false)]
        public async Task TestUpdateClient(int clientId, string updatedName, bool expectedResult)
        {
            var client = new Client
            {
                Name = updatedName,
                Address = "Updated Address",
                City = "Updated City",
                ZipCode = "00000",
                Province = "Updated Province",
                Country = "Updated Country",
                ContactName = "Updated Contact",
                ContactPhone = "0000000000",
                ContactEmail = "updated@example.com"
            };

            var updatedClient = await _clientService.UpdateClientAsync(client, clientId);

            if (expectedResult)
            {
                Assert.IsNotNull(updatedClient);
                Assert.AreEqual(updatedName, updatedClient.Name);
            }
            else
            {
                Assert.IsNull(updatedClient);
            }
        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(999, false)]
        public async Task TestDeleteClient(int clientId, bool expectedResult)
        {
            var result = await _clientService.RemoveClientByIdAsync(clientId);
            Assert.AreEqual(expectedResult, result != null);

            if (result != null)
            {
                var client = await _clientService.GetClientByIdAsync(clientId);
                Assert.IsTrue(client.IsDeleted);
            }
        }
    }
}
