using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EliminarCaracteristicas
{
    public class EliminarCaracteristicasCommandHandler : IRequestHandler<EliminarCaracteristicasCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarCaracteristicasCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Deleted>> Handle(EliminarCaracteristicasCommand request, CancellationToken cancellationToken)
        {
            foreach (var id in request.Ids)
            {
                var result = await _unitOfWork.CaracteristicasRepository.EliminarCaracteristica(id);

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
