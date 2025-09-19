using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class TipoProductoRepository : ITipoProductoRepository
    {
        private readonly CatalogosDbContext _context;
        public TipoProductoRepository(CatalogosDbContext context) {

            _context = context;
        } 
        public async Task<ErrorOr<TipoProducto>> ActualizarTipoProducto(int idTipoProducto, List<string> listaCambios, TipoProducto tipo)
        {
            TipoProducto? config = await _context.TipoProductos.Where(x => x.Id == idTipoProducto).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresTipoProducto.NoEncontrado;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, tipo.GetType().GetProperty(change)?.GetValue(tipo));
            }

            return tipo;
        }

        public async Task<ErrorOr<TipoProducto>> CrearTipoProducto(TipoProducto tipo)
        {
            var tipoCreada = await _context.TipoProductos.AddAsync(tipo);
            return tipoCreada.Entity;
        }

        public async Task<ErrorOr<Deleted>> EliminarTipoProducto(int id)
        {
            var TipoProducto = await _context.TipoProductos.FindAsync(id);

            if (TipoProducto is null)
            {
                return ErroresTipoProducto.NoEncontrado;
            }

            if (TipoProducto.CaracteristicaTipoProductos is not null && TipoProducto.CaracteristicaTipoProductos.Count > 0)
            {
                return ErroresTipoProducto.CaracteristicaRelacionadas;
            }
            _context.TipoProductos.Remove(TipoProducto);
            return Result.Deleted;
        }

        public async Task<ErrorOr<TipoProducto>> ObtenerTipoProductoPorId(int Id)
        {
            var TipoProducto = await _context.TipoProductos.FindAsync(Id);

            if (TipoProducto is null)
            {
                return ErroresTipoProducto.NoEncontrado;
            }

            return TipoProducto;
        }

        public async Task<ErrorOr<List<TipoProducto>>> ObtenerTipoProductos()
        {
            return await _context.TipoProductos.ToListAsync();
        }

        public async Task<ErrorOr<bool>> ValidarExisteProductoRequisito(int idTipoProducto)
        {
            return await _context.ProductoRequisitos.AnyAsync(r => r.IdTipoProducto == idTipoProducto);
        }

        public async Task<ErrorOr<bool>> ValidarTipoProducto(string tipo, int idCategoria, int idInstitucion, int? idTipoProducto = null)
        {

            var resultados = await _context.TipoProductos
                .Where(tp => tp.IdCategoria == idCategoria &&
                             tp.IdInstitucion == idInstitucion &&
                             tp.Id != idTipoProducto)
                .ToListAsync(); // Traes a memoria

            string tipoNormalizado = Validadores.NormalizarString(tipo);
            return resultados.Any(tp => Validadores.NormalizarString(tp.Tipo) == tipoNormalizado);
        }
    }
}
