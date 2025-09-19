using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Imagenes.Commands.CrearImagenes
{

    public class CrearImagenesCommandHandler : IRequestHandler<CrearImagenesCommand, ErrorOr<Dominio.Entidades.Imagenes>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearImagenesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Imagenes>> Handle(CrearImagenesCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.ImagenesRepository.CrearImagen(request.Imagenes);
            await _unitOfWork.Save();
            return result;
        }
    }
}
