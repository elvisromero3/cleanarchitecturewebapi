using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IProvinciaRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.Provincia>>> ObtenerProvincias();
        public Task<ErrorOr<Dominio.Entidades.Provincia>> ObtenerProvinciaPorId(int Id);

        public Task<ErrorOr<Dominio.Entidades.Provincia>> CrearProvincia(Dominio.Entidades.Provincia provincia);

        public Task<ErrorOr<Dominio.Entidades.Provincia>> ActualizarProvincia(int idProvincia, List<string> listaCambios, Dominio.Entidades.Provincia provincia);

        public Task<ErrorOr<Deleted>> EliminarProvincia(int id);

        public Task<ErrorOr<bool>> ValidarProvincia(int idProvincia, string codigo);
        public Task<ErrorOr<bool>> ExisteRelacionConCanton(int idProvincia);
    }
}
