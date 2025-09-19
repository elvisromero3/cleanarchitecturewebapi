using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class ProvinciaRepository : IProvinciaRepository
    {
        private readonly CatalogosDbContext _context;
        public ProvinciaRepository(CatalogosDbContext context) {

            _context = context;
        } 
        public async Task<ErrorOr<Provincia>> ActualizarProvincia(int idProvincia, List<string> listaCambios, Provincia provincia)
        {
            Provincia? config = await _context.Provincias.Where(x => x.Id == idProvincia).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresProvincia.NoEncontrada;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, provincia.GetType().GetProperty(change)?.GetValue(provincia));
            }

            return provincia;
        }

        public async Task<ErrorOr<Provincia>> CrearProvincia(Provincia provincia)
        {
            var provinciaCreada = await _context.Provincias.AddAsync(provincia);
            return provinciaCreada.Entity;
        }

        public async Task<ErrorOr<Deleted>> EliminarProvincia(int id)
        {
            var Provincia = await _context.Provincias.FindAsync(id);

            if (Provincia is null)
            {
                return ErroresProvincia.NoEncontrada;
            }

            _context.Provincias.Remove(Provincia);
            return Result.Deleted;
        }

        public async Task<ErrorOr<Provincia>> ObtenerProvinciaPorId(int Id)
        {
            var Provincia = await _context.Provincias.FindAsync(Id);

            if (Provincia is null)
            {
                return ErroresProvincia.NoEncontrada;
            }

            return Provincia;
        }

        public async Task<ErrorOr<List<Provincia>>> ObtenerProvincias()
        {
            return await _context.Provincias.ToListAsync();
        }

        public async Task<ErrorOr<bool>> ValidarProvincia(int idProvincia,string codigo)
        {
            var provincias = await _context.Provincias
                .Where(p => p.Id != idProvincia)
                .ToListAsync();

            var existe = provincias.Exists(p => Validadores.NormalizarString(p.Codigo) == Validadores.NormalizarString(codigo));

            return existe;    
        }
        public async Task<ErrorOr<bool>> ExisteRelacionConCanton(int idProvincia)
        {
            return await _context.Cantones.AnyAsync(x => x.IdProvincia == idProvincia);
        }
    }
}
