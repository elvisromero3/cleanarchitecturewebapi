using ErrorOr;
using MediatR;

using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Productos.Queries.ObtenerProductosPorId
{
    public class ObtenerProductosPorIdQueryHandler : IRequestHandler<ObtenerProductosPorIdQuery, ErrorOr<Dominio.Entidades.Productos>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerProductosPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Productos>> Handle(ObtenerProductosPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductosRepository.ObtenerProductoPorId(request.IdProducto);
        }
    }
}
