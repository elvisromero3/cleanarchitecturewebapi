using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class VariedadesRepository : IVariedadesRepository
    {
        private readonly CatalogosDbContext _context;

        public VariedadesRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<Variedad>>> ObtenerVariedades()
        {
            return await _context.Variedades.ToListAsync();
        }

        public async Task<ErrorOr<Variedad>> ObtenerVariedadPorId(int id)
        {
            var variedad = await _context.Variedades.FindAsync(id);

            if (variedad is null)
            {
                return ErroresVariedad.NoEncontrado;
            }

            return variedad;
        }

        public async Task<ErrorOr<Variedad>> ObtenerVariedadPorCodigo(string codigo)
        {
            var variedad = await _context.Variedades.AsNoTracking()
                .Where(v => v.Codigo == codigo)
                .FirstOrDefaultAsync();

            if (variedad is null)
            {
                return ErroresVariedad.NoEncontrado;
            }

            return variedad;
        }        

        public async Task<ErrorOr<Variedad>> CrearVariedad(Variedad variedad)
        {
            var variedadCreada = await _context.Variedades.AddAsync(variedad);
            return variedadCreada.Entity;
        }

        public async Task<ErrorOr<Variedad>> EditarVariedad(Variedad variedad, int idVariedad, IEnumerable<string> changeList)
        {
            Variedad? config = await _context.Variedades.FindAsync(idVariedad);

            if (config is null)
            {
                return ErroresVariedad.NoEncontrado;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, variedad.GetType().GetProperty(change)?.GetValue(variedad));
            }

            return config;
        }

        public async Task<ErrorOr<Deleted>> EliminarVariedad(int id)
        {
            var variedad = await _context.Variedades.FindAsync(id);

            if (variedad is null)
            {
                return ErroresVariedad.NoEncontrado;
            }

            _context.Variedades.Remove(variedad);
            return Result.Deleted;
        }

        public async Task<ErrorOr<Deleted>> EliminarVariedad(Variedad variedad)
        {
            await Task.CompletedTask;
            _context.Variedades.Remove(variedad);
            return Result.Deleted;
        }

        public async Task<ErrorOr<bool>> ValidarVariedad(int id, string codigo, string nombre)
        {
            var variedades = await _context.Variedades
                .Where(v => v.Id != id && v.Codigo == codigo)
                .ToListAsync();

            var nombreNormalizado = Validadores.NormalizarString(nombre);
            var existe = variedades.Exists(p => Validadores.NormalizarString(p.Nombre) == nombreNormalizado);
            return existe;            
        }

        public async Task<ErrorOr<bool>> ExisteRelacionConCultivo(int idVariedad)
        {
            return await _context.Cultivos.AnyAsync(x => x.IdVariedad == idVariedad);
        }
    }
}
