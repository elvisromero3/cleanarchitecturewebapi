using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class ProductoRequisitoRepository : IProductoRequisitoRepository
    {
        private readonly CatalogosDbContext _context;
        public ProductoRequisitoRepository(CatalogosDbContext context)
        {
            _context = context;
        }
        public async Task<ErrorOr<ProductoRequisito>> ActualizarProductoRequisito(int idProductoRequisito, List<string> listaCambios, ProductoRequisito productoRequisito)
        {
            ProductoRequisito? config = await _context.ProductoRequisitos.Where(x => x.Id == idProductoRequisito).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresProductoRequisito.NoEncontrada;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, productoRequisito.GetType().GetProperty(change)?.GetValue(productoRequisito));
            }

            return productoRequisito;
        }
        public async Task<ErrorOr<ProductoRequisito>> CrearProductoRequisito(ProductoRequisito productoRequisito)
        {
            var productoRequisitoCreada = await _context.ProductoRequisitos.AddAsync(productoRequisito);
            return productoRequisitoCreada.Entity;
        }
        public async Task<ErrorOr<Deleted>> EliminarProductoRequisito(int id)
        {
            var productoRequisito = await _context.ProductoRequisitos.FindAsync(id);

            if (productoRequisito is null)
            {
                return ErroresProductoRequisito.NoEncontrada;
            }

            _context.ProductoRequisitos.Remove(productoRequisito);
            return Result.Deleted;
        }
        public async Task<ErrorOr<ProductoRequisito>> ObtenerProductoRequisitoPorId(int Id)
        {
            var productoRequisito = await _context.ProductoRequisitos.FindAsync(Id);

            if (productoRequisito is null)
            {
                return ErroresProductoRequisito.NoEncontrada;
            }

            return productoRequisito;
        }

        public async Task<ErrorOr<List<ProductoRequisito>>> ObtenerProductoRequisitos()
        {
            return await _context.ProductoRequisitos.Include(p => p.TipoProducto).ToListAsync();
        }
        public async Task<ErrorOr<bool>> ValidarProductoRequisitos(int idProductoRequisito, int idTipoProducto, int idRequisito)
        {
            return await _context.ProductoRequisitos.AnyAsync(p => 
            p.Id != idProductoRequisito && p.IdTipoProducto == idTipoProducto && p.IdRequisito == idRequisito);

        }
    }
}
