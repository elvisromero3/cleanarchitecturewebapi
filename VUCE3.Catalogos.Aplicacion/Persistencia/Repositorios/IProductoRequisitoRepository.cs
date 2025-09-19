using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IProductoRequisitoRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.ProductoRequisito>>> ObtenerProductoRequisitos();
        public Task<ErrorOr<Dominio.Entidades.ProductoRequisito>> ObtenerProductoRequisitoPorId(int Id);

        public Task<ErrorOr<Dominio.Entidades.ProductoRequisito>> CrearProductoRequisito(Dominio.Entidades.ProductoRequisito productoRequisito);

        public Task<ErrorOr<Dominio.Entidades.ProductoRequisito>> ActualizarProductoRequisito(int idProductoRequisito, List<string> listaCambios, Dominio.Entidades.ProductoRequisito productoRequisito);

        public Task<ErrorOr<Deleted>> EliminarProductoRequisito(int id);
        public Task<ErrorOr<bool>> ValidarProductoRequisitos(int idProductoRequisito, int idTipoProducto, int idRequisito);



    }
}
