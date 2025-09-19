using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class SectoresRepository : ISectoresRepository
    {
        private readonly CatalogosDbContext _context;

        public SectoresRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<Sector>>> ObtenerSectores()
        {
            return await _context.Sectores.ToListAsync();
        }

        public async Task<ErrorOr<Sector>> ObtenerSectorPorId(int Id)
        {
            var sector = await _context.Sectores.FindAsync(Id);

            if (sector is null)
            {
                return ErroresSector.SectorNoEncontrado;
            }

            return sector;
        }

        public async Task<ErrorOr<Sector>> CrearSector(Sector sector)
        {
            var sectorCreado = await _context.Sectores.AddAsync(sector);
            return sectorCreado.Entity;
        }

        public async Task<ErrorOr<Sector>> ActualizarSector(Sector sector, int idSector, List<string> listaCambios)
        {
            Sector? config = await _context.Sectores.FindAsync(idSector);

            if (config is null)
            {
                return ErroresSector.SectorNoEncontrado;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, sector.GetType().GetProperty(change)?.GetValue(sector));
            }

            return config;
        }

        public async Task<ErrorOr<Deleted>> EliminarSector(int id)
        {
            var sector = await _context.Sectores.FindAsync(id);

            if (sector is null)
            {
                return ErroresSector.SectorNoEncontrado;
            }

            _context.Sectores.Remove(sector);
            return Result.Deleted;
        }

        public async Task<ErrorOr<bool>> ValidarSector(int idSector, string nombre, string codigo)
        {
            var sectores = await _context.Sectores
                .Where(p => p.Id != idSector)
                .ToListAsync();

            var nombreNormalizado = Validadores.NormalizarString(nombre);
            var codigoNormalizado = Validadores.NormalizarString(codigo);

            var existe = sectores.Exists(p => Validadores.NormalizarString(p.Nombre) == nombreNormalizado && Validadores.NormalizarString(p.Codigo) == codigoNormalizado);
            return existe;
        }

        public async Task<ErrorOr<Sector>> ObtenerSectorPorNombre(string nombre)
        {
            var sector = await _context.Sectores.Where(p => p.Nombre == nombre).FirstOrDefaultAsync();

            if (sector is null)
            {
                return ErroresSector.SectorNoEncontrado;
            }

            return sector;
        }
    }
}
