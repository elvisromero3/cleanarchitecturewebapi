using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Imagenes.Queries.ObtenerImagenesPorId
{
    public class ObtenerImagenesPorIdQueryHandler : IRequestHandler<ObtenerImagenesPorIdQuery, ErrorOr<Dominio.Entidades.Imagenes>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerImagenesPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Imagenes>> Handle(ObtenerImagenesPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ImagenesRepository.ObtenerImagenPorId(request.Id);
        }
    }
}
