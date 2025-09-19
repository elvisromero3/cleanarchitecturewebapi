using ErrorOr;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IClientesRepository
    {
        public Task<ErrorOr<List<Cliente>>> ObtenerClientes();
        public Task<ErrorOr<Cliente>> ObtenerClientePorId(int id);
        public Task<ErrorOr<Cliente>> CrearCliente(Cliente cliente);
        public Task<ErrorOr<Cliente>> ActualizarCliente(Cliente cliente, int idCliente, IEnumerable<string> changeList);
        public Task<ErrorOr<Deleted>> EliminarCliente(int id);
        public Task<ErrorOr<bool>> ValidarCliente(int idCliente, string codigoCliente, string nombreCliente, int idTipoIdentificacion, string numeroIdentificacion, DateTime fechaVencimiento);
    }
}
