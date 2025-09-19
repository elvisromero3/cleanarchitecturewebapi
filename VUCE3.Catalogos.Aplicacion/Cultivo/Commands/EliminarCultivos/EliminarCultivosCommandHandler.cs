using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EliminarCultivos
{
    public class EliminarCultivosCommandHandler : IRequestHandler<EliminarCultivosCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarCultivosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarCultivosCommand request, CancellationToken cancellationToken)
        {
            foreach (var idCultivo in request.IdsCultivos)
            {
                var result = await _unitOfWork.CultivosRepository.EliminarCultivo(idCultivo);

                if (result.IsError)
                {
                    return result.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Deleted;
        }
    }
}
