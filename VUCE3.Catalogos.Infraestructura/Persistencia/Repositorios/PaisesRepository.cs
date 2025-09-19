using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class PaisesRepository : IPaisesRepository
    {
        private readonly CatalogosDbContext _context;

        public PaisesRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<Pais>>> ObtenerPaises()
        {
            return await _context.Paises.ToListAsync();
        }

        public async Task<ErrorOr<Pais>> ObtenerPaisPorId(int id)
        {
            var pais = await _context.Paises.FindAsync(id);

            if (pais is null)
            {
                return ErroresPaises.NoEncontrado;
            }

            return pais;
        }

        public async Task<ErrorOr<Pais>> CrearPais(Pais pais)
        {
            var paisCreado = await _context.Paises.AddAsync(pais);
            return paisCreado.Entity;
        }

        public async Task<ErrorOr<Pais>> ActualizarPais(Pais pais, int idPais, IEnumerable<string> changeList)
        {
            Pais? config = await _context.Paises.FindAsync(idPais);

            if (config is null)
            {
                return ErroresPaises.NoEncontrado;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, pais.GetType().GetProperty(change)?.GetValue(pais));
            }

            return config;
        }

        public async Task<ErrorOr<Deleted>> EliminarPais(int id)
        {
            var pais = await _context.Paises.FindAsync(id);

            if (pais is null)
            {
                return ErroresPaises.NoEncontrado;
            }

            var paisBloqueComercial = await _context.PaisBloqueComercial.AnyAsync(p => p.IdPais == id);
            if (paisBloqueComercial)
            {
                return ErroresPaises.PerteneceBloqueComercial;
            }

            var paisRequisitos = await _context.Requisitos.AnyAsync(p => p.IdPais == id);
            if (paisRequisitos)
            {
                return ErroresPaises.PerteneceRequisito;
            }

            _context.Paises.Remove(pais);
            return Result.Deleted;
        }

        public async Task<ErrorOr<Pais?>> ValidarPais(int idPais, string nombre, string codigoA2, string codigoNumerico, string codigoC3)
        {
            var paises = await _context.Paises.ToListAsync();

            var existePais = paises.Find(p =>
                (Validadores.NormalizarString(p.Nombre) == Validadores.NormalizarString(nombre) ||
                 p.CodigoA2 == codigoA2 ||
                 p.CodigoNumerico == codigoNumerico ||
                 p.CodigoC3 == codigoC3) &&
                 p.Id != idPais);

            return existePais;
        }
    }
}
