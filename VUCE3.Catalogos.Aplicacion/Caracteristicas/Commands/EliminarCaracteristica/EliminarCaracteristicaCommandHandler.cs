using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EliminarCaracteristica
{
    public class EliminarCaracteristicaCommandHandler : IRequestHandler<EliminarCaracteristicaCommand, ErrorOr<Caracteristica>>
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public EliminarCaracteristicaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;           
        }

        public async Task<ErrorOr<Caracteristica>> Handle(EliminarCaracteristicaCommand command, CancellationToken cancellationToken)
        {
            var caracteristica = await _unitOfWork.CaracteristicasRepository.ObtenerCaracteristicaPorId(command.Id);

            if (caracteristica.IsError)
            {
                return caracteristica.Errors;
            }

            var result = await _unitOfWork.CaracteristicasRepository.EliminarCaracteristica(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return caracteristica;
        }
    }
}
