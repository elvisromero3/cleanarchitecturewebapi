using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisitos;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisitos
{
    public class EliminarRequisitosCommandHandler : IRequestHandler<EliminarRequisitosCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarRequisitosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarRequisitosCommand request, CancellationToken cancellationToken)
        {
            foreach (var idRequisito in request.IdsRequisitos)
            {
                var result = await _unitOfWork.RequisitosRepository.EliminarRequisito(idRequisito);

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
