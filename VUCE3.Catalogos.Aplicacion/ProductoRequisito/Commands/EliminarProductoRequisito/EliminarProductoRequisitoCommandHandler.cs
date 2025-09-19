using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EliminarProductoRequisito
{
    public class EliminarProductoRequisitoCommandHandler : IRequestHandler<EliminarProductoRequisitoCommand, ErrorOr<Dominio.Entidades.ProductoRequisito>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarProductoRequisitoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.ProductoRequisito>> Handle(EliminarProductoRequisitoCommand command, CancellationToken cancellationToken)
        {
            var familia = await _unitOfWork.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(command.Id);

            if (familia.IsError)
            {
                return familia.Errors;
            }

            var result = await _unitOfWork.ProductoRequisitoRepository.EliminarProductoRequisito(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return familia;
        }
    }
}
