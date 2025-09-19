using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogos
{
    public class ObtenerCatalogosQueryHandler : IRequestHandler<ObtenerCatalogosQuery, ErrorOr<List<Dominio.Entidades.Catalogo>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCatalogosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Catalogo>>> Handle(ObtenerCatalogosQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CatalogosRepository.ObtenerCatalogos();
        }
    }
}
