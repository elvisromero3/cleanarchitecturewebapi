using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class PaisBloqueComercialRepository : IPaisBloqueComercialRepository
    {

        private readonly CatalogosDbContext _context;
        public PaisBloqueComercialRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<PaisBloqueComercial>> ActualizarPaisBloqueComercial(int idPaisBloqueComercial, List<string> listaCambios, PaisBloqueComercial paisBloqueComercial)
        {
            PaisBloqueComercial? config = await _context.PaisBloqueComercial.Where(x => x.Id == idPaisBloqueComercial).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresPaisBloqueComercial.NoEncontrada;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, paisBloqueComercial.GetType().GetProperty(change)?.GetValue(paisBloqueComercial));
            }

            return paisBloqueComercial;
        }

        public async Task<ErrorOr<PaisBloqueComercial>> CrearPaisBloqueComercial(PaisBloqueComercial paisBloqueComercial)
        {
            var paisBloqueComercialCreada = await _context.PaisBloqueComercial.AddAsync(paisBloqueComercial);
            return paisBloqueComercialCreada.Entity;
        }

        public async Task<ErrorOr<Deleted>> EliminarPaisBloqueComercial(int id)
        {
            var paisBloqueComercial = await _context.PaisBloqueComercial.FindAsync(id);

            if (paisBloqueComercial is null)
            {
                return ErroresPaisBloqueComercial.NoEncontrada;
            }

            _context.PaisBloqueComercial.Remove(paisBloqueComercial);
            return Result.Deleted;
        }

        public async Task<ErrorOr<PaisBloqueComercial>> ObtenerPaisBloqueComercialPorId(int Id)
        {
            var paisBloqueComercial = await _context.PaisBloqueComercial.FindAsync(Id);

            if (paisBloqueComercial is null)
            {
                return ErroresPaisBloqueComercial.NoEncontrada;
            }

            return paisBloqueComercial;
        }

        public async Task<ErrorOr<List<PaisBloqueComercial>>> ObtenerPaisBloqueComercial()
        {
            return await _context.PaisBloqueComercial.ToListAsync();
        }

        public async Task<ErrorOr<bool>> ValidarPaisBloqueComercial(int idBloqueComercial, int idPais)
        {
            var existe = await _context.PaisBloqueComercial.AnyAsync(u => u.IdBloqueComercial == idBloqueComercial && u.IdPais == idPais);
            return existe;
        }

    } 
}

