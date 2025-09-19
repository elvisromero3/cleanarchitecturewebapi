using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Imagenes.Commands.EditarImagenes
{
    public class EditarImagenesCommandHandler : IRequestHandler<EditarImagenesCommand, ErrorOr<Tuple<Dominio.Entidades.Imagenes, Dominio.Entidades.Imagenes>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarImagenesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.Imagenes, Dominio.Entidades.Imagenes>>> Handle(EditarImagenesCommand command, CancellationToken cancellationToken)
        {
            var imagen = await _unitOfWork.ImagenesRepository.ObtenerImagenPorId(command.IdImagenes);

            if (imagen.IsError)
            {
                return imagen.Errors;
            }

            //se obtiene copia de la imagen antes de aplicar cambios
            Dominio.Entidades.Imagenes imagenAntes = JsonConvert.DeserializeObject<Dominio.Entidades.Imagenes>(
                JsonConvert.SerializeObject(imagen.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? imagen.Value;

            var result = await _unitOfWork.ImagenesRepository.ActualizarImagen(command.Imagenes, command.IdImagenes, command.ListaCambios);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(imagenAntes, imagen.Value);
        }
    }
}
