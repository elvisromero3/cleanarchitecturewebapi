using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Familia.Queries.ObtenerFamiliasPorId;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Familia.Queries.ObtenerFamiliasPorId
{
    public class ObtenerFamiliaPorIdQueryHandler : IRequestHandler<ObtenerFamiliaPorIdQuery, ErrorOr<Dominio.Entidades.Familia>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerFamiliaPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Familia>> Handle(ObtenerFamiliaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FamiliaRepository.ObtenerFamiliaPorId(request.IdFamilia);
        }
    }
}
