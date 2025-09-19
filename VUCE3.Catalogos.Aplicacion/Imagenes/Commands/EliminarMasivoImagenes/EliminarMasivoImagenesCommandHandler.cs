using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.EliminarVariedad;

namespace VUCE3.Catalogos.Aplicacion.Imagenes.Commands.EliminarMasivoImagenes
{
    public class EliminarMasivoImagenesCommandHandler : IRequestHandler<EliminarMasivoImagenesCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarMasivoImagenesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarMasivoImagenesCommand request, CancellationToken cancellationToken)
        {
            foreach (var idImagen in request.IdsImagenes)
            {
                var resultDelete = await _unitOfWork.ImagenesRepository.EliminarImagen(idImagen);

                if (resultDelete.IsError)
                {
                    return resultDelete.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Deleted;
        }
    }
}
