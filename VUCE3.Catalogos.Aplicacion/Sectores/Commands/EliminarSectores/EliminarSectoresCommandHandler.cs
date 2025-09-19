using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Commands.EliminarSectores
{
    public class EliminarSectoresCommandHandler : IRequestHandler<EliminarSectoresCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarSectoresCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarSectoresCommand request, CancellationToken cancellationToken)
        {
            foreach (var idSector in request.IdsSectores)
            {
                var result = await _unitOfWork.SectoresRepository.EliminarSector(idSector);

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
