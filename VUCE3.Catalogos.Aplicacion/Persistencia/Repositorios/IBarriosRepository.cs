using ErrorOr;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IBarriosRepository
    {
        public Task<ErrorOr<List<Barrio>>> ObtenerBarrios();
        public Task<ErrorOr<Barrio>> ObtenerBarrioPorId(int Id);
        public Task<ErrorOr<Barrio>> CrearBarrio(Barrio barrio);
        public Task<ErrorOr<Barrio>> ActualizarBarrio(Barrio barrio, int idBarrio, IEnumerable<string> changeList);
        public Task<ErrorOr<Deleted>> EliminarBarrio(int id);
        public Task<ErrorOr<bool>> ValidarBarrio(int idBarrio, string codigo, int IdDistrito);
    }
}
