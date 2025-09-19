using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Queries.ObtenerAduanaPorId
{
    public class ObtenerAduanaPorIdQueryHandler : IRequestHandler<ObtenerAduanaPorIdQuery, ErrorOr<Aduana>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerAduanaPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Aduana>> Handle(ObtenerAduanaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.AduanasRepository.ObtenerAduanaPorId(request.Id);
        }
    }
}
