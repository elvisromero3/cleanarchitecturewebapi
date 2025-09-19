using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EliminarEstablecimientos
{
    public class EliminarEstablecimientosCommandHandler : IRequestHandler<EliminarEstablecimientosCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarEstablecimientosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarEstablecimientosCommand request, CancellationToken cancellationToken)
        {
            foreach (var idEstablecimiento in request.IdsEstablecimientos)
            {
                var result = await _unitOfWork.EstablecimientosRepository.EliminarEstablecimiento(idEstablecimiento);

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
