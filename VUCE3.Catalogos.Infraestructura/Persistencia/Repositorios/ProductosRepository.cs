using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class ProductosRepository : IProductosRepository
    {
        private readonly CatalogosDbContext _context;
        public ProductosRepository(CatalogosDbContext context)
        {
            _context = context;
        }
        public async Task<ErrorOr<Productos>> ActualizarProductos(int idProductos, List<string> listaCambios, Productos productos)
        {
            Productos? config = await _context.Productos.Where(x => x.Id == idProductos).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresProductos.NoEncontrado;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, productos.GetType().GetProperty(change)?.GetValue(productos));
            }

            return productos;
        }

        public async Task<ErrorOr<Productos>> CrearProductos(Productos productos)
        {
            var productosCreada = await _context.Productos.AddAsync(productos);
            return productosCreada.Entity;
        }

        public async Task<ErrorOr<Deleted>> EliminarProductos(int id)
        {
            var productos = await _context.Productos.FindAsync(id);

            if (productos is null)
            {
                return ErroresProductos.NoEncontrado;
            }

            _context.Productos.Remove(productos);
            return Result.Deleted;
        }

        public async Task<ErrorOr<Productos>> ObtenerProductoPorId(int Id)
        {
            var productos = await _context.Productos.FindAsync(Id);

            if (productos is null)
            {
                return ErroresProductos.NoEncontrado;
            }

            return productos;
        }

        public async Task<ErrorOr<List<Productos>>> ObtenerProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        public async Task<ErrorOr<bool>> ValidarProductos(int idProductos, string nombreComun, string nombreCientifico)
        {
            var productos = await _context.Productos
               .Where(c => c.Id != idProductos)
               .ToListAsync();
            var nombreComunNormalizado = Validadores.NormalizarString(nombreComun);
            var nombreCientificoNormalizado = Validadores.NormalizarString(nombreCientifico);
            var existe = productos.Exists(p => 
            Validadores.NormalizarString(p.NombreComun) == nombreComunNormalizado &&
            Validadores.NormalizarString(p.NombreCientifico) == nombreCientificoNormalizado);
            return existe;
        }

    }
}
