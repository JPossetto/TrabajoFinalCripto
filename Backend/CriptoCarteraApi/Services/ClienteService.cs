using CriptoCarteraApi.DTOs;
using CriptoCarteraApi.Interfaces;
using CriptoCarteraApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CriptoCarteraApi.Services
{
    public class ClienteService : IClienteService
    {
        private readonly AppDbContext _context;

        public ClienteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClienteDTO>> Get()
        {
            var clientes = await _context.Clientes
                .OrderBy(c => c.name)
                .ToListAsync();

            var clientesDto = clientes.Select(c => new ClienteDTO
            {
                id = c.id,
                name = c.name,
                email = c.email
            }).ToList();

            return clientesDto;
        }

        public async Task<ClienteDTO?> Get(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return null;

            return new ClienteDTO
            {
                id = cliente.id,
                name = cliente.name,
                email = cliente.email
            };
        }

        public async Task<ClienteDTO> Post(CrearClienteDTO clienteDto)
        {
            if (string.IsNullOrWhiteSpace(clienteDto.name))
                throw new Exception("El nombre no puede estar vacio");

            if (string.IsNullOrWhiteSpace(clienteDto.email))
                throw new Exception("El email no puede estar vacio");

            // Regex medio simple, pero solo quiero que verifique si se agrega el @ y el . en el email
            if (!Regex.IsMatch(clienteDto.email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new Exception("El email no tiene un formato valido");

            var cliente = new Cliente
            {
                name = clienteDto.name.Trim(),
                email = clienteDto.email.Trim()
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return new ClienteDTO
            {
                id = cliente.id,
                name = cliente.name,
                email = cliente.email
            };
        }
    }
}
