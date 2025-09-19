using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;




using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class EstablecimientosRepository : IEstablecimientosRepository
    {
        private readonly CatalogosDbContext _context;

        public EstablecimientosRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<Establecimiento>> ActualizarEstablecimiento(int id, List<string> listaCambios, Establecimiento establecimiento)
        {
            Establecimiento? config = await _context.Establecimientos.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresEstablecimiento.NoEncontrado;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, establecimiento.GetType().GetProperty(change)?.GetValue(establecimiento));
            }

            return config;
        }

        public async Task<ErrorOr<Establecimiento>> CrearEstablecimiento(Establecimiento establecimiento)
        {
            var creado = await _context.Establecimientos.AddAsync(establecimiento);
            return creado.Entity;
        }

        public async Task<ErrorOr<Deleted>> EliminarEstablecimiento(int id)
        {
            var establecimiento = await _context.Establecimientos.FindAsync(id);

            if (establecimiento is null)
            {
                return ErroresEstablecimiento.NoEncontrado;
            }

            _context.Establecimientos.Remove(establecimiento);

            return Result.Deleted;
        }

        public async Task<ErrorOr<Establecimiento>> ObtenerEstablecimientoPorId(int Id)
        {
            var establecimiento = await _context.Establecimientos.FindAsync(Id);

            if (establecimiento is null)
            {
                return ErroresEstablecimiento.NoEncontrado;
            }

            return establecimiento;
        }

        public async Task<ErrorOr<List<Establecimiento>>> ObtenerEstablecimientos()
        {
            return await _context.Establecimientos.ToListAsync();
        }

        public async Task<ErrorOr<bool>> ValidarEstablecimiento(int id, string numeroCvo)
        {
            var establecimientos = await _context.Establecimientos
                .Where(e => e.Id != id /*&& e.NumeroCvo == numeroCvo*/)
                .ToListAsync();

            var numeroCvoNormalizado = Validadores.NormalizarString(numeroCvo);
            var exist = establecimientos.Any(e => Validadores.NormalizarString(e.NumeroCvo) == numeroCvoNormalizado);

            return exist;
        }
    }
}
