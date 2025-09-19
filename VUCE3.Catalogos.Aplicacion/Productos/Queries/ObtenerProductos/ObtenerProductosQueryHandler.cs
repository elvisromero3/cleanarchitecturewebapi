using ErrorOr;
using MediatR;

using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Productos.Queries.ObtenerProductos
{
    public class ObtenerProductosQueryHandler : IRequestHandler<ObtenerProductosQuery, ErrorOr<List<Dominio.Entidades.Productos>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerProductosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       
        public async Task<ErrorOr<List<Dominio.Entidades.Productos>>> Handle(ObtenerProductosQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductosRepository.ObtenerProductos();
        }
    }
}
