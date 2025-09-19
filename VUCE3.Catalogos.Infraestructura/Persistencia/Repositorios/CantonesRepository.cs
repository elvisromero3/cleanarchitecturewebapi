using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class CantonesRepository : ICantonesRepository
    {
        private readonly CatalogosDbContext _context;
        public CantonesRepository(CatalogosDbContext context) {

            _context = context;
        } 
        public async Task<ErrorOr<Canton>> ActualizarCantones(int idCantones, List<string> listaCambios, Canton cantones)
        {
            Canton? config = await _context.Cantones.Where(x => x.Id == idCantones).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresCanton.NoEncontrado;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, cantones.GetType().GetProperty(change)?.GetValue(cantones));
            }

            return cantones;
        }

        public async Task<ErrorOr<Canton>> CrearCantones(Canton cantones)
        {
            var cantonesCreada = await _context.Cantones.AddAsync(cantones);
            return cantonesCreada.Entity;
        }

        public async Task<ErrorOr<Deleted>> EliminarCantones(int id)
        {
            var Cantones = await _context.Cantones.Include(x=>x.Distritos).Where(x=>x.Id==id).FirstOrDefaultAsync();

            if (Cantones is null)
            {
                return ErroresCanton.NoEncontrado;
            }
            if (Cantones.Distritos.Count > 0)
            {
                return ErroresCanton.DistritosRelacionados;
            }
                       
            _context.Cantones.Remove(Cantones);
            return Result.Deleted;
        }

        public async Task<ErrorOr<Canton>> ObtenerCantonesPorId(int Id)
        {
            var Cantones = await _context.Cantones.FindAsync(Id);

            if (Cantones is null)
            {
                return ErroresCanton.NoEncontrado;
            }

            return Cantones;
        }

        public async Task<ErrorOr<List<Canton>>> ObtenerCantones()
        {
            return await _context.Cantones.ToListAsync();
        }

        public async Task<ErrorOr<bool>> ValidarCantones(int idCanton, string codigo, int idProvincia)
        {
            var cantones = await _context.Cantones
               .Where(p => p.Id != idCanton)
               .ToListAsync();
        
            var existe = cantones.Exists(p => Validadores.NormalizarString(p.Codigo) == Validadores.NormalizarString(codigo)
                                               && p.IdProvincia != idProvincia);
            return existe;         
        }
    }
}
