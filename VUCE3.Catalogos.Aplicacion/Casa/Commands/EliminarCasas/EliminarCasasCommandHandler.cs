using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Casa.Commands.EliminarCasas
{
    public class EliminarCasasCommandHandler : IRequestHandler<EliminarCasasCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarCasasCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarCasasCommand request, CancellationToken cancellationToken)
        {
            foreach (var idCasa in request.IdsCasas)
            {
                var result = await _unitOfWork.CasaRepository.EliminarCasa(idCasa);

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
