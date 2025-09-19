using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Imagenes.Queries.ObtenerImagenes
{
    public class ObtenerImagenesQueryHandler : IRequestHandler<ObtenerImagenesQuery, ErrorOr<List<Dominio.Entidades.Imagenes>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerImagenesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Imagenes>>> Handle(ObtenerImagenesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ImagenesRepository.ObtenerImagenes();
        }
    }
}
