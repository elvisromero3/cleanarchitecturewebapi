using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Imagenes.Commands.EliminarImagenes
{
    public class EliminarImagenesCommandHandler : IRequestHandler<EliminarImagenesCommand, ErrorOr<Dominio.Entidades.Imagenes>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarImagenesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Imagenes>> Handle(EliminarImagenesCommand command, CancellationToken cancellationToken)
        {
            var imagen = await _unitOfWork.ImagenesRepository.ObtenerImagenPorId(command.Id);

            if (imagen.IsError)
            {
                return imagen.Errors;
            }

            var result = await _unitOfWork.ImagenesRepository.EliminarImagen(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return imagen;
        }
    }
}
