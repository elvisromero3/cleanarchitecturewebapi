using ErrorOr;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class CatalogosRepository : ICatalogosRepository
    {
        private readonly CatalogosDbContext _context;

        public CatalogosRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<Catalogo>>> ObtenerCatalogos()
        {
            List<Catalogo> catalogos = await _context.Catalogos.ToListAsync();

            return catalogos;
        }


        public async Task<ErrorOr<List<Catalogo>>> ObtenerCatalogosPorIdInstitucion(int idInstitucion)
        {
            List<Catalogo> catalogos = await _context.Catalogos
                .Join(_context.CatalogoInstitucion, c => c.Id, ci => ci.CatalogoId, (c, ci) => new { c, ci })
                .Where(c => c.ci.InstitucionId == idInstitucion)
                .Select(c => c.c)
                .ToListAsync();

            return catalogos;
        }

        public async Task<ErrorOr<Catalogo>> ObtenerCatalogoPorId(int id)
        {
            var catalogo = await _context.Catalogos.FindAsync(id);

            if (catalogo is null)
            {
                return ErroresCatalogo.NoEncontrado;
            }

            return catalogo;
        }

        public async Task<ErrorOr<Catalogo>> CrearCatalogo(Catalogo catalogo)
        {
            var catalogoCreado = await _context.Catalogos.AddAsync(catalogo);
            return catalogoCreado.Entity;
        }

        public async Task<ErrorOr<Updated>> ActualizarCatalogo(Catalogo catalogo)
        {
            _context.Entry(catalogo).State = EntityState.Modified;
            await Task.CompletedTask;
            return Result.Updated;
        }

        public async Task<ErrorOr<Deleted>> EliminarCatalogo(int id)
        {
            var catalogo = await _context.Catalogos.FindAsync(id);

            if (catalogo is null)
            {
                return ErroresCatalogo.NoEncontrado;
            }

            _context.Catalogos.Remove(catalogo);
            return Result.Deleted;
        }
    }
}
