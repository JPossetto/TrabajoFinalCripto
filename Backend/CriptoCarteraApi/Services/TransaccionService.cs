using CriptoCarteraApi.DTOs;
using CriptoCarteraApi.Interfaces;
using CriptoCarteraApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CriptoCarteraApi.Services
{
    public class TransaccionService : ITransaccionService
    {
        private readonly AppDbContext _context;
        private readonly IPrecioCryptoService _precioCryptoService;

        private readonly List<string> cryptosPermitidas = new()
        {
            "bitcoin",
            "usdc",
            "ethereum"
        };

        public TransaccionService(AppDbContext context, IPrecioCryptoService precioCryptoService)
        {
            _context = context;
            _precioCryptoService = precioCryptoService;
        }

        public async Task<List<TransaccionDTO>> Get(int? clientId)
        {
            var query = _context.Transacciones
                .Include(t => t.cliente)
                .AsQueryable();

            if (clientId != null)
                query = query.Where(t => t.client_id == clientId);

            var transacciones = await query
                .OrderByDescending(t => t.datetime)
                .ToListAsync();

            return transacciones.Select(PasarADto).ToList();
        }

        public async Task<TransaccionDTO?> Get(int id)
        {
            var transaccion = await _context.Transacciones
                .Include(t => t.cliente)
                .FirstOrDefaultAsync(t => t.id == id);

            if (transaccion == null)
                return null;

            return PasarADto(transaccion);
        }

        public async Task<(bool exito, string mensaje, TransaccionDTO? transaccion)> Post(CrearTransaccionDTO transaccionDto)
        {
            var codigo = transaccionDto.crypto_code.ToLower().Trim();
            var accion = transaccionDto.action.ToLower().Trim();

            var validacion = await ValidarTransaccion(codigo, accion, transaccionDto.client_id, transaccionDto.crypto_amount);
            if (!validacion.exito)
                return (false, validacion.mensaje, null);

            if (accion == "sale")
            {
                var disponible = await CalcularCantidadDisponible(transaccionDto.client_id, codigo);

                if (disponible < transaccionDto.crypto_amount)
                    return (false, $"No se puede vender esa cantidad. Disponible: {disponible}", null);
            }

            var precio = await _precioCryptoService.ObtenerPrecio(codigo, accion);
            var total = Math.Round(precio * transaccionDto.crypto_amount, 2);

            var transaccion = new Transaccion
            {
                crypto_code = codigo,
                action = accion,
                client_id = transaccionDto.client_id,
                crypto_amount = transaccionDto.crypto_amount,
                money = total,
                datetime = transaccionDto.datetime
            };

            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();

            var transaccionConCliente = await _context.Transacciones
                .Include(t => t.cliente)
                .FirstAsync(t => t.id == transaccion.id);

            return (true, "Transaccion guardada", PasarADto(transaccionConCliente));
        }

        public async Task<(bool exito, string mensaje)> Patch(int id, EditarTransaccionDTO transaccionDto)
        {
            var transaccion = await _context.Transacciones.FindAsync(id);

            if (transaccion == null)
                return (false, "No encontre la transaccion");

            
            if (transaccionDto.crypto_code != null)
                transaccion.crypto_code = transaccionDto.crypto_code.ToLower().Trim();

            if (transaccionDto.action != null)
                transaccion.action = transaccionDto.action.ToLower().Trim();

            if (transaccionDto.client_id != null)
                transaccion.client_id = transaccionDto.client_id.Value;

            if (transaccionDto.crypto_amount != null)
                transaccion.crypto_amount = transaccionDto.crypto_amount.Value;

            if (transaccionDto.money != null)
                transaccion.money = transaccionDto.money.Value;

            if (transaccionDto.datetime != null)
                transaccion.datetime = transaccionDto.datetime.Value;

            await _context.SaveChangesAsync();
            return (true, "Transaccion editada");
        }

        public async Task<bool> Delete(int id)
        {
            var transaccion = await _context.Transacciones.FindAsync(id);

            if (transaccion == null)
                return false;

            _context.Transacciones.Remove(transaccion);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<(bool exito, string mensaje)> ValidarTransaccion(string codigo, string accion, int clientId, decimal cantidad)
        {
            if (!cryptosPermitidas.Contains(codigo))
                return (false, "La cripto no esta dentro de las opciones permitidas");

            if (accion != "purchase" && accion != "sale")
                return (false, "La accion tiene que ser purchase o sale");

            if (cantidad <= 0)
                return (false, "La cantidad tiene que ser mayor a 0");

            var clienteExiste = await _context.Clientes.AnyAsync(c => c.id == clientId);

            if (!clienteExiste)
                return (false, "El cliente no existe");

            return (true, "Todo bien");
        }

        private async Task<decimal> CalcularCantidadDisponible(int clientId, string codigo)
        {
            var transacciones = await _context.Transacciones
                .Where(t => t.client_id == clientId && t.crypto_code == codigo)
                .ToListAsync();

            var compras = transacciones
                .Where(t => t.action == "purchase")
                .Sum(t => t.crypto_amount);

            var ventas = transacciones
                .Where(t => t.action == "sale")
                .Sum(t => t.crypto_amount);

            return compras - ventas;
        }

        private TransaccionDTO PasarADto(Transaccion transaccion)
        {
            return new TransaccionDTO
            {
                id = transaccion.id,
                crypto_code = transaccion.crypto_code,
                action = transaccion.action,
                client_id = transaccion.client_id,
                client_name = transaccion.cliente?.name ?? "",
                crypto_amount = transaccion.crypto_amount,
                money = transaccion.money,
                datetime = transaccion.datetime
            };
        }
    }
}
