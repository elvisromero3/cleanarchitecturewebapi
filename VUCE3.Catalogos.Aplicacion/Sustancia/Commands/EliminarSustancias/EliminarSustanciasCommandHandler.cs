using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EliminarSustancias
{
    public class EliminarSustanciasCommandHandler : IRequestHandler<EliminarSustanciasCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarSustanciasCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarSustanciasCommand request, CancellationToken cancellationToken)
        {
            foreach (var idSustancia in request.IdsSustancias)
            {
                var sustancia = await _unitOfWork.SustanciasRepository.ObtenerSustanciaPorId(idSustancia);
                if (sustancia.IsError)
                {
                    return sustancia.Errors;
                }
            }
            foreach (var idSustancia in request.IdsSustancias)
            {
                var result = await _unitOfWork.SustanciasRepository.EliminarSustancia(idSustancia);

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
