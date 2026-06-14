using CriptoCarteraApi.DTOs;
using CriptoCarteraApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CriptoCarteraApi.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransaccionService _transaccionService;

        public TransactionsController(ITransaccionService transaccionService)
        {
            _transaccionService = transaccionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransaccionDTO>>> Get([FromQuery] int? clientId)
        {
            var transacciones = await _transaccionService.Get(clientId);
            return Ok(transacciones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransaccionDTO>> Get(int id)
        {
            var transaccion = await _transaccionService.Get(id);

            if (transaccion == null)
                return NotFound();

            return Ok(transaccion);
        }

        [HttpPost]
        public async Task<ActionResult<TransaccionDTO>> Post(CrearTransaccionDTO transaccionDto)
        {
            try
            {
                var resultado = await _transaccionService.Post(transaccionDto);

                if (!resultado.exito)
                    return BadRequest(new { mensaje = resultado.mensaje });

                return CreatedAtAction(nameof(Get), new { id = resultado.transaccion!.id }, resultado.transaccion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, EditarTransaccionDTO transaccionDto)
        {
            var resultado = await _transaccionService.Patch(id, transaccionDto);

            if (!resultado.exito)
                return BadRequest(new { mensaje = resultado.mensaje });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exito = await _transaccionService.Delete(id);

            if (!exito)
                return NotFound();

            return NoContent();
        }
    }
}
