using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class BarriosRepository : IBarriosRepository
    {
        private readonly CatalogosDbContext _context;

        public BarriosRepository(CatalogosDbContext context)
        {
            _context = context;
        }
        public async Task<ErrorOr<List<Barrio>>> ObtenerBarrios()
        {
            return await _context.Barrios
                .Include(x => x.Distrito)
                .ThenInclude(d => d.Canton) 
                .ToListAsync();                
        }

        public async Task<ErrorOr<Barrio>> ObtenerBarrioPorId(int Id)
        {
            var barrio = await _context.Barrios.FindAsync(Id);

            if (barrio is null)
            {
                return ErroresBarrio.BarrioNoEncontrado;
            }

            return barrio;
        }
        public async Task<ErrorOr<Barrio>> CrearBarrio(Barrio barrio)
        {
            var barrioCreado = await _context.Barrios.AddAsync(barrio);
            return barrioCreado.Entity;
        }
        public async Task<ErrorOr<Barrio>> ActualizarBarrio(Barrio barrio, int idBarrio, IEnumerable<string> changeList)
        {
            Barrio? config = await _context.Barrios.FindAsync(idBarrio);

            if (config is null)
            {
                return ErroresBarrio.BarrioNoEncontrado;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, barrio.GetType().GetProperty(change)?.GetValue(barrio));
            }

            return config;
        }
        public async Task<ErrorOr<Deleted>> EliminarBarrio(int id)
        {
            var barrio = await _context.Barrios.FindAsync(id);
            if (barrio is null)
            {
                return ErroresBarrio.BarrioNoEncontrado;
            }

            _context.Barrios.Remove(barrio);
            return Result.Deleted;
        }
        public async Task<ErrorOr<bool>> ValidarBarrio(int idBarrio, string codigo, int IdDistrito)
        {
            var barrios = await _context.Barrios
                 .Where(p => p.Id != idBarrio)
                 .ToListAsync();

            var existe = barrios.Exists(p => Validadores.NormalizarString(p.Codigo) == Validadores.NormalizarString(codigo)
                                             && p.IdDistrito != IdDistrito);

            return existe;
        }
    }
}
