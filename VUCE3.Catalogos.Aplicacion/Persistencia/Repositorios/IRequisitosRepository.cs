using ErrorOr;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IRequisitosRepository
    {
        public Task<ErrorOr<List<Requisito>>> ObtenerRequisitos();
        public Task<ErrorOr<Requisito>> ObtenerRequisitoPorId(int Id);
        public Task<ErrorOr<Requisito>> CrearRequisito(Requisito requisito);
        public Task<ErrorOr<Requisito>> ActualizarRequisito(Requisito requisito, int idRequisito, IEnumerable<string> changeList);
        public Task<ErrorOr<Deleted>> EliminarRequisito(int id);
        public Task<ErrorOr<bool>> ValidarRequisito(int idRequisito, string codigo, string version, int idPais, int idInstitucion);
    }
}
