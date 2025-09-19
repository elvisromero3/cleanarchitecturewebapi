using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class SustanciasRepository : ISustanciasRepository
    {
        private readonly CatalogosDbContext _context;

        public SustanciasRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<Sustancia>>> ObtenerSustancias()
        {
            return await _context.Sustancias.ToListAsync();
        }

        public async Task<ErrorOr<Sustancia>> ObtenerSustanciaPorId(int id)
        {
            var sustancia = await _context.Sustancias.FindAsync(id);
            if (sustancia is null)
            {
                return ErroresSustancia.NoEncontrado;
            }
            return sustancia;
        }
        public async Task<ErrorOr<Sustancia>> CrearSustancia(Sustancia sustancia)
        {
            var sustanciaCreada = await _context.Sustancias.AddAsync(sustancia);
            return sustanciaCreada.Entity;
        }

        public async Task<ErrorOr<Sustancia>> EditarSustancia(Sustancia sustancia, int idSustancia, IEnumerable<string> changeList)
        {
            Sustancia? config = await _context.Sustancias.FindAsync(idSustancia);

            if (config is null)
            {
                return ErroresSustancia.NoEncontrado;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, sustancia.GetType().GetProperty(change)?.GetValue(sustancia));
            }

            return config;
        }

        public async Task<ErrorOr<Deleted>> EliminarSustancia(int id)
        {
            var sustancia = await _context.Sustancias.FindAsync(id);

            if (sustancia is null)
            {
                return ErroresSustancia.NoEncontrado;
            }

            _context.Sustancias.Remove(sustancia);
            return Result.Deleted;
        }

        public async Task<ErrorOr<bool>> ValidarSustancia(int idSustancia, string nombre, string cas, string listaCaq)
        {
            var sustancias = await _context.Sustancias
                .Where(s => s.Id != idSustancia)
                .ToListAsync();

            var normalizarNombre = Validadores.NormalizarString(nombre);
            var normalizarCas = Validadores.NormalizarString(cas);
            var normalizarListaCaq = Validadores.NormalizarString(listaCaq);

            var existe = sustancias.Exists(p =>
                Validadores.NormalizarString(p.Nombre) == normalizarNombre &&
                Validadores.NormalizarString(p.Cas) == normalizarCas &&
                Validadores.NormalizarString(p.ListaCaq) == normalizarListaCaq);

            return existe;
        }
    }
}
