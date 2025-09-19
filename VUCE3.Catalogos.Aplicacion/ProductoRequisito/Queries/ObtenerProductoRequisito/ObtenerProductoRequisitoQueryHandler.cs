using ErrorOr;
using MediatR;

using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.ProductoRequisito.Queries.ObtenerProductoRequisitos
{
    public class ObtenerProductoRequisitosQueryHandler : IRequestHandler<ObtenerProductoRequisitoQuery, ErrorOr<List<Dominio.Entidades.ProductoRequisito>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerProductoRequisitosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       
        public async Task<ErrorOr<List<Dominio.Entidades.ProductoRequisito>>> Handle(ObtenerProductoRequisitoQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductoRequisitoRepository.ObtenerProductoRequisitos();
        }
    }
}
