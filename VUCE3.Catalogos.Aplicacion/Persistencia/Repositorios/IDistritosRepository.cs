using ErrorOr;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IDistritosRepository
    {
        public Task<ErrorOr<List<Distrito>>> ObtenerDistritos();
        public Task<ErrorOr<Distrito>> ObtenerDistritoPorId(int Id);
        public Task<ErrorOr<Distrito>> CrearDistrito(Distrito distrito);
        public Task<ErrorOr<Distrito>> ActualizarDistrito(Distrito distrito, int idDistrito, IEnumerable<string> changeList);
        public Task<ErrorOr<Deleted>> EliminarDistrito(int id);
        public Task<ErrorOr<bool>> ValidarDistrito(int idDistrito, string codigo, int idCanton);
    }
}
