using ErrorOr;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface ITiposAccionesExcepcionesMorosidadRepository
    {
        public Task<ErrorOr<List<TipoAccionExcepcionMorosidad>>> ObtenerTiposAccionesExcepcionesMorosidad();
        public Task<ErrorOr<TipoAccionExcepcionMorosidad>> ObtenerTipoAccionExcepcionMorosidadPorId(int id);
    }
}
