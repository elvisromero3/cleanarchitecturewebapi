using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EliminarExcepcionesMorosidad
{
    public class EliminarExcepcionesMorosidadCommandHandler : IRequestHandler<EliminarExcepcionesMorosidadCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarExcepcionesMorosidadCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarExcepcionesMorosidadCommand request, CancellationToken cancellationToken)
        {
            foreach (var idExcepcionMorosidad in request.IdsExcepcionesMorosidad)
            {
                var excepcion = await _unitOfWork.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(idExcepcionMorosidad);
                if (excepcion.IsError)
                {
                    return excepcion.Errors;
                }

                if (excepcion.Value.IdEstado != ConstantesEstadosExcepcionMorosidad.PROGRAMADA)
                {
                    return ErroresExcepcionMorosidad.EliminacionNoPermitida;
                }                
            }

            foreach (var idExcepcionMorosidad in request.IdsExcepcionesMorosidad)
            {
                    var resultDelete = await _unitOfWork.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(idExcepcionMorosidad);

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
