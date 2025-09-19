using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Variedad.Queries.ObtenerVariedades
{
    public class ObtenerVariedadesQueryHandler : IRequestHandler<ObtenerVariedadesQuery, ErrorOr<List<Dominio.Entidades.Variedad>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerVariedadesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Variedad>>> Handle(ObtenerVariedadesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.VariedadesRepository.ObtenerVariedades();
        }
    }
}
