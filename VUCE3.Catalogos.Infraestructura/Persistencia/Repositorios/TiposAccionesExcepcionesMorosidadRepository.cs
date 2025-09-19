using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class TiposAccionesExcepcionesMorosidadRepository : ITiposAccionesExcepcionesMorosidadRepository
    {
        private readonly CatalogosDbContext _context;
        public TiposAccionesExcepcionesMorosidadRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<TipoAccionExcepcionMorosidad>>> ObtenerTiposAccionesExcepcionesMorosidad()
        {
            var tiposTramites = await _context.TiposAccionesExcepcionesMorosidad.ToListAsync();
            return tiposTramites;
        }

        public async Task<ErrorOr<TipoAccionExcepcionMorosidad>> ObtenerTipoAccionExcepcionMorosidadPorId(int id)
        {
            var tipoTramite = await _context.TiposAccionesExcepcionesMorosidad.FindAsync(id);

            if (tipoTramite is null)
            {
                return ErroresTiposAccionesExcepcionesMorosidad.TipoAccionExcepcionMorosidadNoEncontrado;
            }
            return tipoTramite;
        }
    }
}
