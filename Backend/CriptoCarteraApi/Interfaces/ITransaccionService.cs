using CriptoCarteraApi.DTOs;

namespace CriptoCarteraApi.Interfaces
{
    public interface ITransaccionService
    {
        Task<List<TransaccionDTO>> Get(int? clientId);
        Task<TransaccionDTO?> Get(int id);
        Task<(bool exito, string mensaje, TransaccionDTO? transaccion)> Post(CrearTransaccionDTO transaccionDto);
        Task<(bool exito, string mensaje)> Patch(int id, EditarTransaccionDTO transaccionDto);
        Task<bool> Delete(int id);
    }
}
