using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Casa.Queries.ObtenerCasas
{
    public class ObtenerCasasQueryHandler : IRequestHandler<ObtenerCasasQuery, ErrorOr<List<Dominio.Entidades.Casa>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCasasQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Casa>>> Handle(ObtenerCasasQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CasaRepository.ObtenerCasas();
        }
    }
}
