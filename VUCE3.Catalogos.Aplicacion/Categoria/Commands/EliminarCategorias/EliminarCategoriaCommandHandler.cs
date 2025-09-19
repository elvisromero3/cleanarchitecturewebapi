using ErrorOr;
using MediatR;

using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Categoria.Commands.EliminarCategorias
{
    public class EliminarCategoriaCommandHandler :IRequestHandler<EliminarCategoriasCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarCategoriaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarCategoriasCommand request, CancellationToken cancellationToken)
        {
            var tipoProductos = await _unitOfWork.TipoProductoRepository.ObtenerTipoProductos();

            foreach (var idCategoria in request.IdsCategorias)
            {
                var tipoProducto = tipoProductos.Value.Count(p => p.IdCategoria == idCategoria);
                if (tipoProducto > 0)
                {
                    return ErroresCategoria.TipoProductoRelacionado;
                }
            }

            foreach (var idCategoria in request.IdsCategorias)
            {
                var result = await _unitOfWork.CategoriaRepository.EliminarCategoria(idCategoria);

                if (result.IsError)
                {
                    return result.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Deleted;
        }
    }
}
