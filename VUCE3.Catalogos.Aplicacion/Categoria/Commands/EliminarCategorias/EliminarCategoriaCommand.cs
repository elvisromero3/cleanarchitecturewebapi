using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Categoria.Commands.EliminarCategorias
{
    public class EliminarCategoriasCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsCategorias { get; set; } = null!;
    }
}
