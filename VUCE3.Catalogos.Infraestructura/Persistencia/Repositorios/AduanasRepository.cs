using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class AduanasRepository : IAduanasRepository
    {
        private readonly CatalogosDbContext _context;

        public AduanasRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<Aduana>>> ObtenerAduanas()
        {
            return await _context.Aduanas.ToListAsync();
        }

        public async Task<ErrorOr<Aduana>> ObtenerAduanaPorId(int Id)
        {
            var aduana = await _context.Aduanas.FindAsync(Id);

            if (aduana is null)
            {
                return ErroresAduana.NoEncontrada;
            }

            return aduana;
        }

        public async Task<ErrorOr<Aduana>> CrearAduana(Aduana aduana)
        {
            var aduanaCreada = await _context.Aduanas.AddAsync(aduana);
            return aduanaCreada.Entity;
        }

        public async Task<ErrorOr<Aduana>> ActualizarAduana(Aduana aduana, int idAduana, IEnumerable<string> changeList)
        {
            Aduana? config = await _context.Aduanas.FindAsync(idAduana);

            if (config is null)
            {
                return ErroresAduana.NoEncontrada;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, aduana.GetType().GetProperty(change)?.GetValue(aduana));
            }

            return config;
        }

        public async Task<ErrorOr<Deleted>> EliminarAduana(int id)
        {
            var aduana = await _context.Aduanas.FindAsync(id);

            if (aduana is null)
            {
                return ErroresAduana.NoEncontrada;
            }

            _context.Aduanas.Remove(aduana);
            return Result.Deleted;
        }

        public async Task<ErrorOr<bool>> ValidarAduana(Aduana aduana)
        {
            var aduanas = await _context.Aduanas
                .Where(p => p.Id != aduana.Id)
                .ToListAsync();

            var nombreNormalizado = Validadores.NormalizarString(aduana.Nombre);
            var existe = aduanas.Exists(p => Validadores.NormalizarString(p.Nombre) == nombreNormalizado);
            return existe;            
        }
    }
}
