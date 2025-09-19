using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Productos.Commands.EliminarProducto
{
    public class EliminarProductoCommandHandler : IRequestHandler<EliminarProductoCommand, ErrorOr<Dominio.Entidades.Productos>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarProductoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Productos>> Handle(EliminarProductoCommand command, CancellationToken cancellationToken)
        {
            var productos = await _unitOfWork.ProductosRepository.ObtenerProductoPorId(command.Id);

            if (productos.IsError)
            {
                return productos.Errors;
            }

            var result = await _unitOfWork.ProductosRepository.EliminarProductos(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return productos;
        }
    }
}
