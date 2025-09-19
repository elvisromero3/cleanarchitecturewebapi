using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IPaisesRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.Pais>>> ObtenerPaises();
        public Task<ErrorOr<Dominio.Entidades.Pais>> ObtenerPaisPorId(int id);
        public Task<ErrorOr<Dominio.Entidades.Pais>> CrearPais(Dominio.Entidades.Pais pais);
        public Task<ErrorOr<Dominio.Entidades.Pais>> ActualizarPais(Dominio.Entidades.Pais pais, int idPais, IEnumerable<string> changeList);
        public Task<ErrorOr<Deleted>> EliminarPais(int id);
        public Task<ErrorOr<Dominio.Entidades.Pais?>> ValidarPais(int idPais,string nombre, string codigoA2, string codigoNumerico, string codigoC3);
    }
}
