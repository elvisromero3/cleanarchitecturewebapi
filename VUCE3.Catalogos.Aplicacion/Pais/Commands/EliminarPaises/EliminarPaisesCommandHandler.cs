using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Pais.Commands.EliminarPaises
{
    public class EliminarPaisesCommandHandler : IRequestHandler<EliminarPaisesCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarPaisesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarPaisesCommand request, CancellationToken cancellationToken)
        {
            foreach(var idPais in request.IdsPaises)
            {
                var resultDelete = await _unitOfWork.PaisesRepository.EliminarPais(idPais);

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
