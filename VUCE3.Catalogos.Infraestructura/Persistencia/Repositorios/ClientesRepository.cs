using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class ClientesRepository :IClientesRepository
    {
        private readonly CatalogosDbContext _context;
        public ClientesRepository(CatalogosDbContext context)
        {

            _context = context;
        }

        public async Task<ErrorOr<List<Cliente>>> ObtenerClientes()
        {
            return await _context.Clientes.ToListAsync();

        }

        public async Task<ErrorOr<Cliente>> ObtenerClientePorId(int id) 
        {

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente is null)
            {
                return ErroresCliente.NoEncontrado;
            }
            return cliente;
        }

        public async Task<ErrorOr<Cliente>> CrearCliente(Cliente cliente) {
            
            var clienteNuevo = await _context.Clientes.AddAsync(cliente);

            return clienteNuevo.Entity;
        }

        public async Task<ErrorOr<Cliente>> ActualizarCliente(Cliente cliente, int idCliente, IEnumerable<string> changeList)
        {
            Cliente? config = await _context.Clientes.Where(x => x.Id == idCliente).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresCliente.NoEncontrado;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, cliente.GetType().GetProperty(change)?.GetValue(cliente));
            }

            return cliente;
        }

        public async Task<ErrorOr<Deleted>> EliminarCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente is null)
            {
                return ErroresCliente.NoEncontrado;
            }
            _context.Clientes.Remove(cliente);

            return Result.Deleted;
        }

        public async Task<ErrorOr<bool>> ValidarCliente(int idCliente, string codigoCliente, string nombreCliente, int idTipoIdentificacion, string numeroIdentificacion, DateTime fechaVencimiento)
        {
            var clientes = await _context.Clientes
                .Where(c => c.Id != idCliente && c.CodigoCliente == codigoCliente && c.NumeroIdentificacion == numeroIdentificacion && c.IdTipoIdentificacion == idTipoIdentificacion && c.FechaVencimiento.Date == fechaVencimiento.Date)
            .ToListAsync();

            var nombreNormalizado = Validadores.NormalizarString(nombreCliente);
            var existe = clientes.Exists(p => Validadores.NormalizarString(p.NombreCliente) == nombreNormalizado);
            return existe;
        }
    }
}
