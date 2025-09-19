using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class CultivosRepository : ICultivosRepository
    {
        private readonly CatalogosDbContext _context;

        public CultivosRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<Cultivo>>> ObtenerCultivos()
        {
            return await _context.Cultivos
                .Include(c => c.Variedad)
                .ToListAsync();
        }

        public async Task<ErrorOr<Cultivo>> ObtenerCultivoPorId(int id)
        {
            var cultivo = await _context.Cultivos.FindAsync(id);

            if (cultivo is null)
            {
                return ErroresCultivo.NoEncontrado;
            }

            return cultivo;
        }

        public async Task<ErrorOr<Cultivo>> CrearCultivo(Cultivo cultivo)
        {
            var cultivoCreado = await _context.Cultivos.AddAsync(cultivo);
            return cultivoCreado.Entity;
        }

        public async Task<ErrorOr<Cultivo>> EditarCultivo(Cultivo cultivo, int idCultivo, IEnumerable<string> changeList)
        {
            Cultivo? config = await _context.Cultivos.FindAsync(idCultivo);

            if (config is null)
            {
                return ErroresCultivo.NoEncontrado;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, cultivo.GetType().GetProperty(change)?.GetValue(cultivo));
            }

            return config;
        }

        public async Task<ErrorOr<Deleted>> EliminarCultivo(int id)
        {
            var cultivo = await _context.Cultivos.FindAsync(id);

            if (cultivo is null)
            {
                return ErroresCultivo.NoEncontrado;
            }

            _context.Cultivos.Remove(cultivo);
            return Result.Deleted;
        }

        public async Task<ErrorOr<bool>> ValidarCultivo(int idCultivo, string codigo, string nombre, string nombreCientifico)
        {
            var cultivos = await _context.Cultivos
               .Where(c => c.Id != idCultivo && c.Codigo == codigo)
               .ToListAsync();

            var nombreNormalizado = Validadores.NormalizarString(nombre);
            var nombreCientificoNormalizado = Validadores.NormalizarString(nombreCientifico);
            var existe = cultivos.Exists(p => 
                Validadores.NormalizarString(p.Nombre) == nombreNormalizado &&
                Validadores.NormalizarString(p.NombreCientifico) == nombreCientificoNormalizado
            );
            return existe;
        }

    }
}
