using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Commands.EliminarBarrio
{
    public class EliminarBarrioCommandHandler : IRequestHandler<EliminarBarrioCommand, ErrorOr<Barrio>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarBarrioCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Barrio>> Handle(EliminarBarrioCommand command, CancellationToken cancellationToken)
        {
            var barrio = await _unitOfWork.BarriosRepository.ObtenerBarrioPorId(command.Id);
            if (barrio.IsError)
            {
                return barrio.Errors;
            }

            var result = await _unitOfWork.BarriosRepository.EliminarBarrio(command.Id);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return barrio;
        }
    }
}
