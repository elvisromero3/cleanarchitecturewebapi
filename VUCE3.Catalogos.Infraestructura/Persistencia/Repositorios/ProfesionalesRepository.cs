using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class ProfesionalesRepository : IProfesionalesRepository
    {
        private readonly CatalogosDbContext _context;

        public ProfesionalesRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<Profesional>>> ObtenerProfesionales()
        {
            return await _context.Profesionales.ToListAsync();
        }

        public async Task<ErrorOr<Profesional>> ObtenerProfesionalPorId(int id)
        {
            var profesional = await _context.Profesionales.FindAsync(id);

            if (profesional is null)
            {
                return ErroresProfesionales.NoEncontrado;
            }

            return profesional;
        }

        public async Task<ErrorOr<Profesional>> CrearProfesional(Profesional profesional)
        {
            var profesionalCreado = await _context.Profesionales.AddAsync(profesional);
            return profesionalCreado.Entity;
        }

        public async Task<ErrorOr<Profesional>> ActualizarProfesional(Profesional profesional, int idProfesional, IEnumerable<string> changeList)
        {
            Profesional? config = await _context.Profesionales.FindAsync(idProfesional);

            if (config is null)
            {
                return ErroresProfesionales.NoEncontrado;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, profesional.GetType().GetProperty(change)?.GetValue(profesional));
            }

            return config;
        }

        public async Task<ErrorOr<Deleted>> EliminarProfesional(int id)
        {
            var profesional = await _context.Profesionales.FindAsync(id);

            if (profesional is null)
            {
                return ErroresProfesionales.NoEncontrado;
            }

            _context.Profesionales.Remove(profesional);
            return Result.Deleted;
        }    
        
        public async Task<ErrorOr<bool>> ValidarProfesional(int idProfesional,string numeroIdentificacion, int idInstitucion)
        {
            return await _context.Profesionales.AnyAsync(p => p.Id != idProfesional && p.NumeroIdentificacion == numeroIdentificacion && p.IdInstitucion == idInstitucion);
        }

        public async Task<ErrorOr<List<Profesional>>> ObtenerProfesionalesPorIdInstitucion(int idInstitucion)
        {
            return await _context.Profesionales.Where(x=> x.IdInstitucion == idInstitucion).ToListAsync();
        }
    }
}
