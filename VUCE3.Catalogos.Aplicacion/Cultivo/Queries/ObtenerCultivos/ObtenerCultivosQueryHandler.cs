using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivos
{
    public class ObtenerCultivosQueryHandler : IRequestHandler<ObtenerCultivosQuery, ErrorOr<List<Dominio.Entidades.Cultivo>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCultivosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Cultivo>>> Handle(ObtenerCultivosQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CultivosRepository.ObtenerCultivos();
        }
    }
}
