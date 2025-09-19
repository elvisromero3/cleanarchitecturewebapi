using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EliminarProductoRequisitos;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.ProductoRequisito.Command.EliminarProdcutoRequisitos
{
    public class EliminarProductoRequisitosCommandHandler : IRequestHandler<EliminarProductoRequisitosCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarProductoRequisitosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarProductoRequisitosCommand request, CancellationToken cancellationToken)
        {
            foreach (var idRequisito in request.IdsRequisitos)
            {
                var result = await _unitOfWork.ProductoRequisitoRepository.EliminarProductoRequisito(idRequisito);

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
