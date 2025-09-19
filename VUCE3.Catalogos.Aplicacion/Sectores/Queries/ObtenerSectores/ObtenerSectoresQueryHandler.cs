using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Queries.ObtenerSectores
{
    public class ObtenerSectoresQueryHandler : IRequestHandler<ObtenerSectoresQuery, ErrorOr<List<Sector>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerSectoresQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Sector>>> Handle(ObtenerSectoresQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SectoresRepository.ObtenerSectores();
        }
    }
}
