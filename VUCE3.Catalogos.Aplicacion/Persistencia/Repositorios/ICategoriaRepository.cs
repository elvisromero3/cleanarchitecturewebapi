using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface ICategoriaRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.Categoria>>> ObtenerCategorias();
        public Task<ErrorOr<Dominio.Entidades.Categoria>> ObtenerCategoriaPorId(int Id);

        public Task<ErrorOr<Dominio.Entidades.Categoria>> CrearCategoria(Dominio.Entidades.Categoria Categoria);

        public Task<ErrorOr<Dominio.Entidades.Categoria>> ActualizarCategoria(int idCategoria, List<string> listaCambios, Dominio.Entidades.Categoria Categoria);

        public Task<ErrorOr<Deleted>> EliminarCategoria(int id);

        public Task<ErrorOr<bool>> ValidarCategoria(int idCategoria, string nombre, int idInstitucion);
    }
}
