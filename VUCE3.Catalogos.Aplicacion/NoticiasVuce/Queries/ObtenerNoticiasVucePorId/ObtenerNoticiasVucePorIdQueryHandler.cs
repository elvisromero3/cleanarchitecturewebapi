using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Pais.Queries.ObtenerPaisPorId;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.NoticiasVuce.Queries.ObtenerNoticiasVucePorId
{
    public class ObtenerNoticiasVucePorIdQueryHandler : IRequestHandler<ObtenerNoticiasVucePorIdQuery, ErrorOr<Dominio.Entidades.NoticiasVuce>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerNoticiasVucePorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.NoticiasVuce>> Handle(ObtenerNoticiasVucePorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.NoticiasVuceRepository.ObtenerNoticiasVucePorId(request.Id);
        }
    }
}
