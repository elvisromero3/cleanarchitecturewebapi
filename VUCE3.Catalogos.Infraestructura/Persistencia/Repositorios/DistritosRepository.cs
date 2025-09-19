using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class DistritosRepository : IDistritosRepository
    {
        private readonly CatalogosDbContext _context;

        public DistritosRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<Distrito>>> ObtenerDistritos()
        {
            return await _context.Distritos
                .Include(x => x.Canton)
                .ToListAsync();
        }

        public async Task<ErrorOr<Distrito>> ObtenerDistritoPorId(int Id)
        {
            var distrito = await _context.Distritos.FindAsync(Id);

            if (distrito is null)
            {
                return ErroresDistrito.DistritoNoEncontrado;
            }

            return distrito;
        }

        public async Task<ErrorOr<Distrito>> CrearDistrito(Distrito distrito)
        {
            var distritoCreado = await _context.Distritos.AddAsync(distrito);
            return distritoCreado.Entity;
        }

        public async Task<ErrorOr<Distrito>> ActualizarDistrito(Distrito distrito, int idDistrito, IEnumerable<string> changeList)
        {
            Distrito? config = await _context.Distritos.FindAsync(idDistrito);

            if (config is null)
            {
                return ErroresDistrito.DistritoNoEncontrado;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, distrito.GetType().GetProperty(change)?.GetValue(distrito));
            }

            return config;
        }

        public async Task<ErrorOr<Deleted>> EliminarDistrito(int id)
        {
            var distrito = await _context.Distritos.Include(x => x.Barrios).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (distrito is null)
            {
                return ErroresDistrito.DistritoNoEncontrado;
            }
            if (distrito.Barrios.Count > 0)
            {
                return ErroresDistrito.BarriosRelacionados;
            }

            _context.Distritos.Remove(distrito);
            return Result.Deleted;
        }

        public async Task<ErrorOr<bool>> ValidarDistrito(int idDistrito, string codigo, int idCanton)
        {
            var distritos = await _context.Distritos
                .Where(p => p.Id != idDistrito)
                .ToListAsync();

            var existe = distritos.Exists(p => Validadores.NormalizarString(p.Codigo) == Validadores.NormalizarString(codigo)
                                                && p.IdCanton != idCanton);

            return existe;
        }
    }
}
