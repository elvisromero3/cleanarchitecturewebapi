using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class CasaRepository : ICasaRepository
    {
        private readonly CatalogosDbContext _context;
        public CasaRepository(CatalogosDbContext context) {

            _context = context;
        } 
        public async Task<ErrorOr<Casa>> ActualizarCasa(int idCasa, List<string> listaCambios, Casa casa)
        {
            Casa? config = await _context.Casas.Where(x => x.Id == idCasa).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresCasa.NoEncontrada;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, casa.GetType().GetProperty(change)?.GetValue(casa));
            }

            return casa;
        }

        public async Task<ErrorOr<Casa>> CrearCasa(Casa casa)
        {
            var casaCreada = await _context.Casas.AddAsync(casa);
            return casaCreada.Entity;
        }

        public async Task<ErrorOr<Deleted>> EliminarCasa(int id)
        {
            var casa = await _context.Casas.FindAsync(id);

            if (casa is null)
            {
                return ErroresCasa.NoEncontrada;
            }

            _context.Casas.Remove(casa);
            return Result.Deleted;
        }

        public async Task<ErrorOr<Casa>> ObtenerCasaPorId(int Id)
        {
            var casa = await _context.Casas.FindAsync(Id);

            if (casa is null)
            {
                return ErroresCasa.NoEncontrada;
            }

            return casa;
        }

        public async Task<ErrorOr<List<Casa>>> ObtenerCasas()
        {
            return await _context.Casas.ToListAsync();
        }

        public async Task<ErrorOr<bool>> ValidarCasa(int idCasa, string codigo, string nombre)
        {
            var casas = await _context.Casas
                .Where(c => c.Id != idCasa && c.Codigo == codigo)
                .ToListAsync();

            var nombreNormalizado = Validadores.NormalizarString(nombre);
            var existe = casas.Exists(p => Validadores.NormalizarString(p.Nombre) == nombreNormalizado);

            return existe;            
        }
    }
}
