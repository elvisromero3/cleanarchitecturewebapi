using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class CaracteristicaTipoProductoRepository : ICaracteristicaTipoProductoRepository
    {
        private readonly CatalogosDbContext _context;
        public CaracteristicaTipoProductoRepository(CatalogosDbContext context)
        {
            _context = context;
        }
        public async Task<ErrorOr<CaracteristicaTipoProducto>> ActualizarCaracteristicaTipoProducto(int idCaracteristicaTipoProducto, List<string> listaCambios, CaracteristicaTipoProducto caracteristicaTipoProducto)
        {
            CaracteristicaTipoProducto? config = await _context.CaracteristicaTipoProductos.Where(x => x.Id == idCaracteristicaTipoProducto).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresCaracteristicaTipoProducto.NoEncontrada;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, caracteristicaTipoProducto.GetType().GetProperty(change)?.GetValue(caracteristicaTipoProducto));
            }

            return caracteristicaTipoProducto;
        }
        public async Task<ErrorOr<CaracteristicaTipoProducto>> CrearCaracteristicaTipoProducto(CaracteristicaTipoProducto caracteristicaTipoProducto)
        {
            var caracteristicaTipoProductoCreada = await _context.CaracteristicaTipoProductos.AddAsync(caracteristicaTipoProducto);
            return caracteristicaTipoProductoCreada.Entity;
        }
        public async Task<ErrorOr<Deleted>> EliminarCaracteristicaTipoProducto(int id)
        {
            var caracteristicaTipoProducto = await _context.CaracteristicaTipoProductos.FindAsync(id);

            if (caracteristicaTipoProducto is null)
            {
                return ErroresCaracteristicaTipoProducto.NoEncontrada;
            }

            _context.CaracteristicaTipoProductos.Remove(caracteristicaTipoProducto);
            return Result.Deleted;
        }
        public async Task<ErrorOr<CaracteristicaTipoProducto>> ObtenerCaracteristicaTipoProductoPorId(int Id)
        {
            var caracteristicaTipoProducto = await _context.CaracteristicaTipoProductos.FindAsync(Id);

            if (caracteristicaTipoProducto is null)
            {
                return ErroresCaracteristicaTipoProducto.NoEncontrada;
            }

            return caracteristicaTipoProducto;
        }

        public async Task<ErrorOr<List<CaracteristicaTipoProducto>>> ObtenerCaracteristicaTipoProductos()
        {
            return await _context.CaracteristicaTipoProductos.Include(x=>x.Caracteristica).ToListAsync();
        }

        public async Task<ErrorOr<bool>> ValidarCaracteristicaTipoProducto(int idTipoProducto, int idCaracterisitica)
        {
            return await _context.CaracteristicaTipoProductos.AnyAsync(x => x.IdTipoProducto == idTipoProducto &&  x.IdCaracteristica == idCaracterisitica );
        }

        public async Task<ErrorOr<bool>> VerificaExistenciaPorTipoProducto(int idTipoProducto)
        {
            var existe = await _context.CaracteristicaTipoProductos.AnyAsync(x=>x.IdTipoProducto==idTipoProducto);

            return existe;
        }

    }
}
