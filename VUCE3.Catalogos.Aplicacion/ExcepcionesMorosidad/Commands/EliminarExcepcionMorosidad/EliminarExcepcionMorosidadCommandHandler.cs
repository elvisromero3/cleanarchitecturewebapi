using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EliminarExcepcionMorosidad
{
    public class EliminarExcepcionMorosidadCommandHandler : IRequestHandler<EliminarExcepcionMorosidadCommand, ErrorOr<ExcepcionMorosidad>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarExcepcionMorosidadCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<ExcepcionMorosidad>> Handle(EliminarExcepcionMorosidadCommand command, CancellationToken cancellationToken)
        {
            var excepcion = await _unitOfWork.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(command.Id);
            if (excepcion.IsError)
            {
                return excepcion.Errors;
            }
            
            var result = await _unitOfWork.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(command.Id);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return excepcion;
        }
    }
}
