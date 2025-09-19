using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogos;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogosPorIdInstitucion
{
    public class ObtenerCatalogosPorIdInstitucionQueryHandler : IRequestHandler<ObtenerCatalogosPorIdInstitucionQuery, ErrorOr<List<Dominio.Entidades.Catalogo>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCatalogosPorIdInstitucionQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Catalogo>>> Handle(ObtenerCatalogosPorIdInstitucionQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CatalogosRepository.ObtenerCatalogosPorIdInstitucion(request.IdInstitucion);
        }
    }
}
