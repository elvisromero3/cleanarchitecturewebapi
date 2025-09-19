using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly CatalogosDbContext _context;
        public CategoriaRepository(CatalogosDbContext context)
        {

            _context = context;
        }

        public async Task<ErrorOr<List<Categoria>>> ObtenerCategorias()
        {
            return await _context.Categorias.ToListAsync();
        }

        public async Task<ErrorOr<Categoria>> ObtenerCategoriaPorId(int Id)
        {
            var Categoria = await _context.Categorias.FindAsync(Id);

            if (Categoria is null)
            {
                return ErroresCategoria.NoEncontrada;
            }

            return Categoria;
        }

        public async Task<ErrorOr<Categoria>> CrearCategoria(Categoria Categoria)
        {
            var CategoriaCreada = await _context.Categorias.AddAsync(Categoria);
            return CategoriaCreada.Entity;
        }

        public async Task<ErrorOr<Categoria>> ActualizarCategoria(int idCategoria, List<string> listaCambios, Categoria Categoria)
        {
            Categoria? config = await _context.Categorias.Where(x => x.Id == idCategoria).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresCategoria.NoEncontrada;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, Categoria.GetType().GetProperty(change)?.GetValue(Categoria));
            }

            return Categoria;
        }

        public async Task<ErrorOr<Deleted>> EliminarCategoria(int id)
        {
            var Categoria = await _context.Categorias.FindAsync(id);

            if (Categoria is null)
            {
                return ErroresCategoria.NoEncontrada;
            }
            
            _context.Categorias.Remove(Categoria);
            return Result.Deleted;
        }

        public async Task<ErrorOr<bool>> ValidarCategoria(int idCategoria, string nombre, int idInstitucion)
        {
            var nombreNormalizado = Validadores.NormalizarString(nombre);
            var Categorias = await _context.Categorias
                .Where(c => c.Id != idCategoria)
                .ToListAsync();
            
            var existe = Categorias.Exists(p => Validadores.NormalizarString(p.Nombre) == nombreNormalizado && p.IdInstitucion==idInstitucion);

            return existe;
        }
    }
}
