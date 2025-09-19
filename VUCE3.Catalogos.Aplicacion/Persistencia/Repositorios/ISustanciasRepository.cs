using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface ISustanciasRepository
    {
        Task<ErrorOr<List<Dominio.Entidades.Sustancia>>> ObtenerSustancias();
        Task<ErrorOr<Dominio.Entidades.Sustancia>> ObtenerSustanciaPorId(int id);
        Task<ErrorOr<Dominio.Entidades.Sustancia>> CrearSustancia(Dominio.Entidades.Sustancia sustancia);
        Task<ErrorOr<Dominio.Entidades.Sustancia>> EditarSustancia(Dominio.Entidades.Sustancia sustancia, int idSustancia, IEnumerable<string> changeList);
        Task<ErrorOr<Deleted>> EliminarSustancia(int id);
        Task<ErrorOr<bool>> ValidarSustancia(int idSustancia, string nombre, string cas, string listaCaq);
    }
}
