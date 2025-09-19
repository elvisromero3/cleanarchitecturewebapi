using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IProductosRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.Productos>>> ObtenerProductos();
        public Task<ErrorOr<Dominio.Entidades.Productos>> ObtenerProductoPorId(int Id);

        public Task<ErrorOr<Dominio.Entidades.Productos>> CrearProductos(Dominio.Entidades.Productos producto);

        public Task<ErrorOr<Dominio.Entidades.Productos>> ActualizarProductos(int idProductos, List<string> listaCambios, Dominio.Entidades.Productos producto);

        public Task<ErrorOr<Deleted>> EliminarProductos(int id);

        public Task<ErrorOr<bool>> ValidarProductos(int idProductos, string nombreComun, string nombreCientifico);
    }
}
