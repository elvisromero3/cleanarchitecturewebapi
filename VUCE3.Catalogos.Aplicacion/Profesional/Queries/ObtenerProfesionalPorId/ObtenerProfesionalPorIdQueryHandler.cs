using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Profesional.Queries.ObtenerProfesionalPorId
{
    public class ObtenerProfesionalPorIdQueryHandler : IRequestHandler<ObtenerProfesionalPorIdQuery, ErrorOr<Dominio.Entidades.Profesional>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerProfesionalPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Profesional>> Handle(ObtenerProfesionalPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProfesionalesRepository.ObtenerProfesionalPorId(request.Id);
        }
    }
}
