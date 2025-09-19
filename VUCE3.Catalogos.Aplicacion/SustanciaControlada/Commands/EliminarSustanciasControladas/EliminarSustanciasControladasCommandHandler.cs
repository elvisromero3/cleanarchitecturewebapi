using ErrorOr;
using MediatR;

using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.EliminarSustanciasControladas
{
    public class EliminarSustanciasControladasCommandHandler :IRequestHandler<EliminarSustanciasControladasCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarSustanciasControladasCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarSustanciasControladasCommand request, CancellationToken cancellationToken)
        {
            foreach (var idSustanciaControlada in request.IdsSustanciasControladas)
            {
                var result = await _unitOfWork.SustanciaControladaRepository.EliminarSustanciaControlada(idSustanciaControlada);

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
