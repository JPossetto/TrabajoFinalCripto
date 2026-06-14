using CriptoCarteraApi.DTOs;

namespace CriptoCarteraApi.Interfaces
{
    public interface IClienteService
    {
        Task<List<ClienteDTO>> Get();
        Task<ClienteDTO?> Get(int id);
        Task<ClienteDTO> Post(CrearClienteDTO clienteDto);
    }
}
