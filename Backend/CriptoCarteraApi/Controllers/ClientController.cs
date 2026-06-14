using CriptoCarteraApi.DTOs;
using CriptoCarteraApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CriptoCarteraApi.Controllers
{
    [ApiController]
    [Route("client")]
    public class ClientController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> Get()
        {
            var clientes = await _clienteService.Get();
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDTO>> Get(int id)
        {
            var cliente = await _clienteService.Get(id);

            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDTO>> Post(CrearClienteDTO clienteDto)
        {
            try
            {
                var cliente = await _clienteService.Post(clienteDto);
                return CreatedAtAction(nameof(Get), new { id = cliente.id }, cliente);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
