using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Profesional.Queries.ObtenerProfesionales
{
    public class ObtenerProfesionalesQueryHandler : IRequestHandler<ObtenerProfesionalesQuery, ErrorOr<List<Dominio.Entidades.Profesional>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerProfesionalesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Profesional>>> Handle(ObtenerProfesionalesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProfesionalesRepository.ObtenerProfesionales();
        }
    }
}
