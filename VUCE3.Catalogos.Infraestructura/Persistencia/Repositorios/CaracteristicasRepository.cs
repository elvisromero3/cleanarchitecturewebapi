using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class CaracteristicasRepository : ICaracteristicasRepository
    {
        private readonly CatalogosDbContext _context;

        public CaracteristicasRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<Caracteristica>>> ObtenerCaracteristicas()
        {
            return await _context.Caracteristicas.ToListAsync();
        }

        public async Task<ErrorOr<Caracteristica>> ObtenerCaracteristicaPorId(int Id)
        {
            var caracteristica = await _context.Caracteristicas.FindAsync(Id);

            if (caracteristica is null)
            {
                return ErroresCaracteristica.CaracteristicaNoEncontrada;
            }

            return caracteristica;
        }

        public async Task<ErrorOr<Caracteristica>> CrearCaracteristica(Caracteristica caracteristica)
        {
            var caracteristicaCreada = await _context.Caracteristicas.AddAsync(caracteristica);
            return caracteristicaCreada.Entity;
        }

        public async Task<ErrorOr<Caracteristica>> ActualizarCaracteristica(Caracteristica caracteristica, int idCaracteristica, IEnumerable<string> changeList)
        {
            Caracteristica? config = await _context.Caracteristicas.FindAsync(idCaracteristica);

            if (config is null)
            {
                return ErroresCaracteristica.CaracteristicaNoEncontrada;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, caracteristica.GetType().GetProperty(change)?.GetValue(caracteristica));
            }

            return config;
        }

        public async Task<ErrorOr<Deleted>> EliminarCaracteristica(int id)
        {
            var caracteristica = await _context.Caracteristicas.FindAsync(id);

            if (caracteristica is null)
            {
                return ErroresCaracteristica.CaracteristicaNoEncontrada;
            }


            var caracteristicasTipoProducto = await _context.CaracteristicaTipoProductos.AsNoTracking()
                .Where(v => v.IdCaracteristica == caracteristica.Id)
                .FirstOrDefaultAsync();

            if (caracteristicasTipoProducto is not null)
            {
                return ErroresCaracteristica.TipoProductoRelacionado;
            }


            _context.Caracteristicas.Remove(caracteristica);
            return Result.Deleted;
        }

        public async Task<ErrorOr<bool>> ValidarCaracteristica(int idCaracteristica, int idInstitucion, string nombre)
        {
            var caracteristicas = await _context.Caracteristicas
                .Where(p => p.Id != idCaracteristica)
                .ToListAsync();

            var nombreNormalizado = Validadores.NormalizarString(nombre);
            var existe = caracteristicas.Exists(p => p.IdInstitucion == idInstitucion && Validadores.NormalizarString(p.Nombre) == nombreNormalizado);
            return existe;
        }
    }
}
