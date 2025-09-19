using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IProfesionalesRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.Profesional>>> ObtenerProfesionales();
        public Task<ErrorOr<List<Dominio.Entidades.Profesional>>> ObtenerProfesionalesPorIdInstitucion(int idInstitucion);
        public Task<ErrorOr<Dominio.Entidades.Profesional>> ObtenerProfesionalPorId(int id);
        public Task<ErrorOr<Dominio.Entidades.Profesional>> CrearProfesional(Dominio.Entidades.Profesional profesional);
        public Task<ErrorOr<Dominio.Entidades.Profesional>> ActualizarProfesional(Dominio.Entidades.Profesional profesional, int idProfesional, IEnumerable<string> changeList);
        public Task<ErrorOr<Deleted>> EliminarProfesional(int id);
        public Task<ErrorOr<bool>> ValidarProfesional(int idProfesional, string numeroIdentificacion, int idInstitucion);
    }
}
