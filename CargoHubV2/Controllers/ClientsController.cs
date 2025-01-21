using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CargohubV2.Models;
using CargohubV2.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CargohubV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : Controller
    {
        private readonly ClientsService _clientsService;

        public ClientsController(ClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        // GET: api/Clients
        [HttpGet("byAmount/{amount}")]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClients(int amount)
        {
            if (amount <= 0)
            {
                return BadRequest(new { Message = "Invalid amount. It must be a positive integer." });
            }
            var clients = await _clientsService.GetAllClientsAsync(amount);
            return Ok(clients);
        }

        // GET: api/Clients/{id}
        [HttpGet("{id}")] // Route parameter
        public async Task<ActionResult<Client>> GetClientById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid client ID. It must be a positive integer." });
            }
            var client = await _clientsService.GetClientByIdAsync(id);
            if (client == null)
            {
                return NoContent();
            }
            return Ok(client);
        }
        [HttpGet("Email/{email}")] // Route parameter
        public async Task<ActionResult<Client>> GetClientByEmail([FromRoute] string email)
        {
            var client = await _clientsService.GetClientByEmailAsync(email);
            if (client == null)
            {
                return NoContent();
            }
            return Ok(client);
        }

        // POST: api/Clients/Add
        [HttpPost("Add")]
        public async Task<IActionResult> CreateClient([FromBody] Client client)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Return validation errors
            }
            if (_clientsService.GetClientByEmailAsync(client.ContactEmail).Result != null)
            {
                return BadRequest("Client already exists");
            }
            var createdClient = await _clientsService.CreateClientAsync(client);
            return CreatedAtAction(nameof(GetClientById), new { id = createdClient.Id }, createdClient);
        }

        //PUT: api/Clients/Update/{id}
        [HttpPut("Update/{id}")] // Route parameter
        public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid client ID. It must be a positive integer." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedClient = await _clientsService.UpdateClientAsync(client, id);
            if (updatedClient == null)
            {
                return NoContent();
            }
            return Ok(updatedClient);
        }

        // DElETE: api/Clients/Delete/{id}
        [HttpDelete("Delete/{id}")] // Route parameter
        public async Task<IActionResult> RemoveClientById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid client ID. It must be a positive integer." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var client = await _clientsService.RemoveClientByIdAsync(id);
            if (client == null)
            {
                return NoContent();
            }
            return Ok("Client deleted successfully");
        }

        [HttpDelete("Delete/Email/{email}")] // Route parameter
        public async Task<IActionResult> RemoveClientByEmail(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var client = await _clientsService.RemoveClientByEmailAsync(email);
            if (client == null)
            {
                return NoContent();
            }
            return Ok("Client deleted successfully");
        }

    }
}
