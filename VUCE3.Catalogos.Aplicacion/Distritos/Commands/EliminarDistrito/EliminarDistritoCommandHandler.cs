using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Commands.EliminarDistrito
{
    public class EliminarDistritoCommandHandler : IRequestHandler<EliminarDistritoCommand, ErrorOr<Distrito>>
    {
        private readonly IUnitOfWork _unitOfWork;
     
        public EliminarDistritoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Distrito>> Handle(EliminarDistritoCommand command, CancellationToken cancellationToken)
        {
            var distrito = await _unitOfWork.DistritosRepository.ObtenerDistritoPorId(command.Id);
            if (distrito.IsError)
            {
                return distrito.Errors;
            }
           
            var result = await _unitOfWork.DistritosRepository.EliminarDistrito(command.Id);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return distrito;
        }
    }
}
