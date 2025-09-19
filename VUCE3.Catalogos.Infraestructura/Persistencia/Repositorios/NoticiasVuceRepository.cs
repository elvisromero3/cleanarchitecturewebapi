using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class NoticiasVuceRepository : INoticiasVuceRepository
    {
        private readonly CatalogosDbContext _context;
        public NoticiasVuceRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<NoticiasVuce>> ActualizarNoticiasVuce(NoticiasVuce noticiasVuce, int idNoticiasVuce, IEnumerable<string> changeList)
        {
            NoticiasVuce? config = await _context.NoticiasVuces.FindAsync(idNoticiasVuce);

            if (config is null)
            {
                return ErroresNoticiasVuce.NoEncontrada;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, noticiasVuce.GetType().GetProperty(change)?.GetValue(noticiasVuce));
            }

            return config;
        }

        public async Task<ErrorOr<NoticiasVuce>> CrearNoticiasVuce(NoticiasVuce noticiasVuce)
        {
            var noticiasVuceCreado = await _context.NoticiasVuces.AddAsync(noticiasVuce);
            return noticiasVuceCreado.Entity;
        }

        public async Task<ErrorOr<Deleted>> EliminarNoticiasVuce(int id)
        {
            var noticiasVuce = await _context.NoticiasVuces.FindAsync(id);

            if (noticiasVuce is null)
            {
                return ErroresNoticiasVuce.NoEncontrada;
            }

            _context.NoticiasVuces.Remove(noticiasVuce);
            return Result.Deleted;
        }

        public async Task<ErrorOr<List<NoticiasVuce>>> ObtenerNoticiasVuce()
        {
            return await _context.NoticiasVuces.ToListAsync();
        }

        public async Task<ErrorOr<NoticiasVuce>> ObtenerNoticiasVucePorId(int id)
        {
            var noticiasVuce = await _context.NoticiasVuces.FindAsync(id);

            if (noticiasVuce is null)
            {
                return ErroresNoticiasVuce.NoEncontrada;
            }

            return noticiasVuce;
        }

        public async Task<ErrorOr<bool>> ValidarNoticiasVuce(string titulo, string texto, string? enlace, string tituloIngles, string textoIngles)
        {
            var noticias = await _context.NoticiasVuces
                .ToListAsync();

            var tituloNormalizado = Validadores.NormalizarString(titulo);
            var tituloInglesNormalizado = Validadores.NormalizarString(tituloIngles);
            var textoNormalizado = Validadores.NormalizarString(texto);
            var textoInglesNormalizado = Validadores.NormalizarString(textoIngles);

            var existe = noticias.Exists(p => Validadores.NormalizarString(p.Titulo) == tituloNormalizado
                && Validadores.NormalizarString(p.TituloIngles) == tituloInglesNormalizado
                && Validadores.NormalizarString(p.Texto) == textoNormalizado
                && p.Enlace == (enlace == null ? string.Empty : enlace) 
                && Validadores.NormalizarString(p.TextoIngles) == textoInglesNormalizado);

            return existe;
        }
    }
}
