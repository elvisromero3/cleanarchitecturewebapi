using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Variedad.Queries.ObtenerVariedadPorId
{
    public class ObtenerVariedadPorIdQueryHandler : IRequestHandler<ObtenerVariedadPorIdQuery, ErrorOr<Dominio.Entidades.Variedad>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerVariedadPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Variedad>> Handle(ObtenerVariedadPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.VariedadesRepository.ObtenerVariedadPorId(request.Id);
        }

    }
}
