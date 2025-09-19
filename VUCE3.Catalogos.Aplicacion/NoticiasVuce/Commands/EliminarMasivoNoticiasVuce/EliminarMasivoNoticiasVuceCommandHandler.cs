using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EliminarMasivoNoticiasVuce
{
    public class EliminarMasivoNoticiasVuceCommandHandler : IRequestHandler<EliminarMasivoNoticiasVuceCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarMasivoNoticiasVuceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarMasivoNoticiasVuceCommand request, CancellationToken cancellationToken)
        {
            foreach(var idNoticia in request.IdsNoticias)
            {
                var resultDelete = await _unitOfWork.NoticiasVuceRepository.EliminarNoticiasVuce(idNoticia);

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
