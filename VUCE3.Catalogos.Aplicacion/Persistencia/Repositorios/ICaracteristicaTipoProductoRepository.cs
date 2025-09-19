using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface ICaracteristicaTipoProductoRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.CaracteristicaTipoProducto>>> ObtenerCaracteristicaTipoProductos();
        public Task<ErrorOr<Dominio.Entidades.CaracteristicaTipoProducto>> ObtenerCaracteristicaTipoProductoPorId(int Id);

        public Task<ErrorOr<Dominio.Entidades.CaracteristicaTipoProducto>> CrearCaracteristicaTipoProducto(Dominio.Entidades.CaracteristicaTipoProducto caracteristicaTipoProducto);

        public Task<ErrorOr<Dominio.Entidades.CaracteristicaTipoProducto>> ActualizarCaracteristicaTipoProducto(int idCaracteristicaTipoProducto, List<string> listaCambios, Dominio.Entidades.CaracteristicaTipoProducto caracteristicaTipoProducto);

        public Task<ErrorOr<Deleted>> EliminarCaracteristicaTipoProducto(int id);
        public Task<ErrorOr<bool>> ValidarCaracteristicaTipoProducto(int idTipoProducto, int idCaracterisitica);
        public Task<ErrorOr<bool>> VerificaExistenciaPorTipoProducto(int idTipoProducto);
    }
}
