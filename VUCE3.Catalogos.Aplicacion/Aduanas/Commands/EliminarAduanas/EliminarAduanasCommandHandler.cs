using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.EliminarVariedad;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EliminarAduanas
{
    public class EliminarAduanasCommandHandler : IRequestHandler<EliminarAduanasCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarAduanasCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarAduanasCommand request, CancellationToken cancellationToken)
        {
            foreach (var idAduana in request.IdsAduanas)
            {
                var resultDelete = await _unitOfWork.AduanasRepository.EliminarAduana(idAduana);

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
