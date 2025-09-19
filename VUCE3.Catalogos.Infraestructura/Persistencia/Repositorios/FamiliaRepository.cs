using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class FamiliaRepository : IFamiliaRepository
    {
        private readonly CatalogosDbContext _context;
        public FamiliaRepository(CatalogosDbContext context)
        {
            _context = context;
        }
        public async Task<ErrorOr<Familia>> ActualizarFamilia(int idFamilia, List<string> listaCambios, Familia familia)
        {
            Familia? config = await _context.Familias.Where(x => x.Id == idFamilia).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresFamilia.NoEncontrada;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, familia.GetType().GetProperty(change)?.GetValue(familia));
            }

            return familia;
        }
        public async Task<ErrorOr<Familia>> CrearFamilia(Familia familia)
        {
            var familiaCreada = await _context.Familias.AddAsync(familia);
            return familiaCreada.Entity;
        }
        public async Task<ErrorOr<Deleted>> EliminarFamilia(int id)
        {
            var familia = await _context.Familias.FindAsync(id);

            if (familia is null)
            {
                return ErroresFamilia.NoEncontrada;
            }

            _context.Familias.Remove(familia);
            return Result.Deleted;
        }
        public async Task<ErrorOr<Familia>> ObtenerFamiliaPorId(int Id)
        {
            var familia = await _context.Familias.FindAsync(Id);

            if (familia is null)
            {
                return ErroresFamilia.NoEncontrada;
            }

            return familia;
        }

        public async Task<ErrorOr<List<Familia>>> ObtenerFamilias()
        {
            return await _context.Familias.ToListAsync();
        }

        public async Task<ErrorOr<bool>> ValidarFamilia(int idFamilia, string nombre)
        {
            var familias = await _context.Familias
                .Where(c => c.Id != idFamilia)
                .ToListAsync();
            var nombreNormalizado = Validadores.NormalizarString(nombre);
            var existe = familias.Exists(p => Validadores.NormalizarString(p.Nombre) == nombreNormalizado);
            return existe;
        }

    }
}
