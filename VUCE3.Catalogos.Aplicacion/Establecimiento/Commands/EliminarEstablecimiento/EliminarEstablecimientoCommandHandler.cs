using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EliminarEstablecimiento
{
    public class EliminarEstablecimientoCommandHandler : IRequestHandler<EliminarEstablecimientoCommand, ErrorOr<Dominio.Entidades.Establecimiento>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarEstablecimientoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Establecimiento>> Handle(EliminarEstablecimientoCommand request, CancellationToken cancellationToken)
        {
            var establecimiento = await _unitOfWork.EstablecimientosRepository.ObtenerEstablecimientoPorId(request.Id);
            if(establecimiento.IsError)
            {
                return establecimiento.Errors;
            }

            var result = await _unitOfWork.EstablecimientosRepository.EliminarEstablecimiento(request.Id);
            if(result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return establecimiento;
        }
    }
}
