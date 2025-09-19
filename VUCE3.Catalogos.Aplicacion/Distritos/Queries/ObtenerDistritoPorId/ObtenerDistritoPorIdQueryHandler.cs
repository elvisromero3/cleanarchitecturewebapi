using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Queries.ObtenerDistritoPorId
{
    public class ObtenerDistritoPorIdQueryHandler : IRequestHandler<ObtenerDistritoPorIdQuery, ErrorOr<Distrito>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerDistritoPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Distrito>> Handle(ObtenerDistritoPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.DistritosRepository.ObtenerDistritoPorId(request.Id);
        }
    }
}
