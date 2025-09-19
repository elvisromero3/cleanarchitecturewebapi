using ErrorOr;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface ISectoresRepository
    {
        public Task<ErrorOr<List<Sector>>> ObtenerSectores();

        public Task<ErrorOr<Sector>> ObtenerSectorPorId(int Id);

        public Task<ErrorOr<Sector>> CrearSector(Sector sector);

        public Task<ErrorOr<Sector>> ActualizarSector(Sector sector, int idSector, List<string> listaCambios);

        public Task<ErrorOr<Deleted>> EliminarSector(int id);

        public Task<ErrorOr<bool>> ValidarSector(int idSector, string nombre, string codigo);

        public Task<ErrorOr<Sector>> ObtenerSectorPorNombre(string nombre);
    }
}
