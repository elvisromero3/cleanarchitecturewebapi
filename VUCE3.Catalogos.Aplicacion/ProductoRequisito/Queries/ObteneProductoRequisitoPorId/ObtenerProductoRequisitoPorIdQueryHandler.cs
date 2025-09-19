using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Casa.Queries.ObtenerCasaPorId;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.ProductoRequisito.Queries.ObtenerProductoRequisitosPorId
{
    public class ObtenerProductoRequisitoPorIdQueryHandler : IRequestHandler<ObtenerProductoRequisitoPorIdQuery, ErrorOr<Dominio.Entidades.ProductoRequisito>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerProductoRequisitoPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.ProductoRequisito>> Handle(ObtenerProductoRequisitoPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(request.IdProductoRequisito);
        }
    }
}
