using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivoPorId
{
    public class ObtenerCultivoPorIdQueryHandler : IRequestHandler<ObtenerCultivoPorIdQuery, ErrorOr<Dominio.Entidades.Cultivo>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCultivoPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Cultivo>> Handle(ObtenerCultivoPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CultivosRepository.ObtenerCultivoPorId(request.IdCultivo);
        }
    }
}
