using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Cantones.Queries.ObtenerCantones
{
    public class ObtenerCantonesQueryHandler : IRequestHandler<ObtenerCantonesQuery, ErrorOr<List<Dominio.Entidades.Canton>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCantonesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Canton>>> Handle(ObtenerCantonesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CantonesRepository.ObtenerCantones();
        }
    }
}
