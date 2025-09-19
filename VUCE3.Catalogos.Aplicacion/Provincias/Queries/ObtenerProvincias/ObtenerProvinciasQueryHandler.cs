using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Provincia.Queries.ObtenerProvincias
{
    public class ObtenerProvinciasQueryHandler : IRequestHandler<ObtenerProvinciasQuery, ErrorOr<List<Dominio.Entidades.Provincia>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerProvinciasQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Provincia>>> Handle(ObtenerProvinciasQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProvinciaRepository.ObtenerProvincias();
        }
    }
}
