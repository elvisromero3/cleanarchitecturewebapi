using ErrorOr;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IEstadosExcepcionesMorosidadRepository
    {
        public Task<ErrorOr<List<EstadoExcepcionMorosidad>>> ObtenerEstadosExcepcionesMorosidad();
        public Task<ErrorOr<EstadoExcepcionMorosidad>> ObtenerEstadoExcepcionMorosidadPorId(int id);
    }
}
