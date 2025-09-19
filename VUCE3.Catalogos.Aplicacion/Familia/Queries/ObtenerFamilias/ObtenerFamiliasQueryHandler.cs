using ErrorOr;
using MediatR;

using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Familia.Queries.ObtenerFamilias
{
    public class ObtenerFamiliasQueryHandler : IRequestHandler<ObtenerFamiliasQuery, ErrorOr<List<Dominio.Entidades.Familia>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerFamiliasQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       
        public async Task<ErrorOr<List<Dominio.Entidades.Familia>>> Handle(ObtenerFamiliasQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FamiliaRepository.ObtenerFamilias();
        }
    }
}
