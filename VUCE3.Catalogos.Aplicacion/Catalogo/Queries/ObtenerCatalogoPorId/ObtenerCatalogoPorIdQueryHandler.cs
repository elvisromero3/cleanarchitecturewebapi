using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogoPorId
{
    public class ObtenerCatalogoPorIdQueryHandler : IRequestHandler<ObtenerCatalogoPorIdQuery, ErrorOr<Dominio.Entidades.Catalogo>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCatalogoPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Catalogo>> Handle(ObtenerCatalogoPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CatalogosRepository.ObtenerCatalogoPorId(request.IdCatalogo);
        }
    }
}
