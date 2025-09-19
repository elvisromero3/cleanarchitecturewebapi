using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class BloqueComercialRepository : IBloqueComercialRepository
    {
        private readonly CatalogosDbContext _context;
        public BloqueComercialRepository(CatalogosDbContext context) {

            _context = context;
        } 
        public async Task<ErrorOr<BloqueComercial>> ActualizarBloqueComercial(int idBloqueComercial, List<string> listaCambios, BloqueComercial bloquecomercial)
        {
            BloqueComercial? config = await _context.BloquesComerciales.Where(x => x.Id == idBloqueComercial).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresBloqueComercial.NoEncontrada;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, bloquecomercial.GetType().GetProperty(change)?.GetValue(bloquecomercial));
            }

            return bloquecomercial;
        }

        public async Task<ErrorOr<BloqueComercial>> CrearBloqueComercial(BloqueComercial bloquecomercial)
        {
            var provinciaCreada = await _context.BloquesComerciales.AddAsync(bloquecomercial);
            return provinciaCreada.Entity;
        }

        public async Task<ErrorOr<Deleted>> EliminarBloqueComercial(int id)
        {
            var BloqueComercial = await _context.BloquesComerciales.FindAsync(id);

            var paisesBloqueComercial = await _context.PaisBloqueComercial
                .Where(pbc => pbc.IdBloqueComercial == id)
                .ToListAsync();

            if (paisesBloqueComercial is not null)
            {
                foreach (var paisBloqueComercial in paisesBloqueComercial)
                {
                    _context.PaisBloqueComercial.Remove(paisBloqueComercial);
                }
            }

            if (BloqueComercial is null)
            {
                return ErroresBloqueComercial.NoEncontrada;
            }

            _context.BloquesComerciales.Remove(BloqueComercial);
            return Result.Deleted;
        }

        public async Task<ErrorOr<BloqueComercial>> ObtenerBloqueComercialPorId(int Id)
        {
            var BloqueComercial = await _context.BloquesComerciales.FindAsync(Id);

            if (BloqueComercial is null)
            {
                return ErroresBloqueComercial.NoEncontrada;
            }

            return BloqueComercial;
        }

        public async Task<ErrorOr<List<BloqueComercial>>> ObtenerBloquesComerciales()
        {
            return await _context.BloquesComerciales.ToListAsync();
        }

        public async Task<ErrorOr<bool>> ValidarBloqueComercial(int idBloqueComercial,string nombre)
        {
            var bloques = await _context.BloquesComerciales
                .Where(p => p.Id != idBloqueComercial)
                .ToListAsync();

            var nombreNormalizado = Validadores.NormalizarString(nombre);
            var existe = bloques.Exists(p => Validadores.NormalizarString(p.Nombre) == nombreNormalizado);

            return existe;
        }
    }
}
