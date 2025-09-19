using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.CrearProductoRequisito
{
    public class CrearProductoRequisitoCommandHandler : IRequestHandler<CrearProductoRequisitoCommand, ErrorOr<Dominio.Entidades.ProductoRequisito>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearProductoRequisitoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.ProductoRequisito>> Handle(CrearProductoRequisitoCommand request, CancellationToken cancellationToken)
        {
            var existeTipo = await _unitOfWork.TipoProductoRepository.ObtenerTipoProductoPorId(request.ProductoRequisito.IdTipoProducto);
            if( existeTipo.Value is null)
            {
                return ErroresProductoRequisito.TipoProductoNoEncontrada;
            }
            var existeRequisito = await _unitOfWork.RequisitosRepository.ObtenerRequisitoPorId(request.ProductoRequisito.IdRequisito);
            if (existeRequisito.Value is null)
            {
                return ErroresProductoRequisito.RequisitoNoEncontrado;
            }

            var productoRequisitoExistente = await _unitOfWork.ProductoRequisitoRepository
                .ValidarProductoRequisitos(0, request.ProductoRequisito.IdTipoProducto, request.ProductoRequisito.IdRequisito);
            if (productoRequisitoExistente.Value)
            {
                return ErroresProductoRequisito.ProductoRequisitoDuplicados;
            }

            var result = await _unitOfWork.ProductoRequisitoRepository.CrearProductoRequisito(request.ProductoRequisito);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }

    }
}
