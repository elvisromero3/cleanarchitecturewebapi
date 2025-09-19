using ErrorOr;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface ICaracteristicasRepository
    {
        public Task<ErrorOr<List<Caracteristica>>> ObtenerCaracteristicas();
        public Task<ErrorOr<Caracteristica>> ObtenerCaracteristicaPorId(int Id);
        public Task<ErrorOr<Caracteristica>> CrearCaracteristica(Caracteristica caracteristica);
        public Task<ErrorOr<Caracteristica>> ActualizarCaracteristica(Caracteristica caracteristica, int idCaracteristica, IEnumerable<string> changeList);
        public Task<ErrorOr<Deleted>> EliminarCaracteristica(int id);
        public Task<ErrorOr<bool>> ValidarCaracteristica(int idCaracteristica, int idInstitucion, string nombre);
    }
}
