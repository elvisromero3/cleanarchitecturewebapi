using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class EstadosExcepcionesMorosidadRepository : IEstadosExcepcionesMorosidadRepository
    {
        private readonly CatalogosDbContext _context;
        public EstadosExcepcionesMorosidadRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<EstadoExcepcionMorosidad>>> ObtenerEstadosExcepcionesMorosidad()
        {
            var estados = await _context.EstadosExcepcionesMorosidad.ToListAsync();
            return estados;
        }

        public async Task<ErrorOr<EstadoExcepcionMorosidad>> ObtenerEstadoExcepcionMorosidadPorId(int id)
        {
            var estado = await _context.EstadosExcepcionesMorosidad.FindAsync(id);

            if (estado is null)
            {
                return ErroresEstadosExcepcionesMorosidad.EstadoExcepcionMorosidadNoEncontrado;
            }
            return estado;
        }
    }
}
