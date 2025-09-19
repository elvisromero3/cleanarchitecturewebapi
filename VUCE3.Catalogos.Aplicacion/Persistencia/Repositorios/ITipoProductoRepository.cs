using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface ITipoProductoRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.TipoProducto>>> ObtenerTipoProductos();
        public Task<ErrorOr<Dominio.Entidades.TipoProducto>> ObtenerTipoProductoPorId(int Id);

        public Task<ErrorOr<Dominio.Entidades.TipoProducto>> CrearTipoProducto(Dominio.Entidades.TipoProducto tipo);

        public Task<ErrorOr<Dominio.Entidades.TipoProducto>> ActualizarTipoProducto(int idTipoProducto, List<string> listaCambios, Dominio.Entidades.TipoProducto tipo);

        public Task<ErrorOr<Deleted>> EliminarTipoProducto(int id);

        public Task<ErrorOr<bool>> ValidarTipoProducto(string tipo, int idCategoria, int idInstitucion, int? idTipoProducto = null);
        public Task<ErrorOr<bool>> ValidarExisteProductoRequisito(int idTipoProducto);

    }
}
