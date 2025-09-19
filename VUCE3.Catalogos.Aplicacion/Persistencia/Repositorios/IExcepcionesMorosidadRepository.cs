using ErrorOr;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IExcepcionesMorosidadRepository
    {
        public Task<ErrorOr<List<ExcepcionMorosidad>>> ObtenerExcepcionesMorosidad();
        public Task<ErrorOr<ExcepcionMorosidad>> ObtenerExcepcionMorosidadPorId(int Id);
        public Task<ErrorOr<ExcepcionMorosidad>> CrearExcepcionMorosidad(ExcepcionMorosidad excepcionMorosidad);
        public Task<ErrorOr<ExcepcionMorosidad>> ActualizarExcepcionMorosidad(int idExcepcionMorosidad, List<string> changeList, ExcepcionMorosidad excepcionMorosidad);
        public Task<ErrorOr<Deleted>> EliminarExcepcionMorosidad(int id);
        public Task<ErrorOr<bool>> ValidarExcepcionMorosidad(ExcepcionMorosidad excepcionMorosidad, bool excluirProgramadas);
    }
}