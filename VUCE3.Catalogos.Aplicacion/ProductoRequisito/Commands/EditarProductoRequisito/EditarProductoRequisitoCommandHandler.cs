using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EditarProductoRequisito;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EditarProductoRequisito
{
    public class EditarProductoRequisitoCommandHandler : IRequestHandler<EditarProductoRequisitoCommand, ErrorOr<Tuple<Dominio.Entidades.ProductoRequisito, Dominio.Entidades.ProductoRequisito>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarProductoRequisitoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.ProductoRequisito, Dominio.Entidades.ProductoRequisito>>> Handle(EditarProductoRequisitoCommand command, CancellationToken cancellationToken)
        {
            // Obtener ProductoRequisito por Id
            var productoRequisito = await _unitOfWork.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(command.IdProductoRequisito);
            if (productoRequisito.IsError)
            {
                return productoRequisito.Errors;
            }

            var comprobarDuplicado = false;
            var comprobarIdTipoProducto = productoRequisito.Value.IdTipoProducto;
            var comprobarIdRequisito = productoRequisito.Value.IdRequisito;

            if (command.ListaCambios.Contains("IdTipoProducto"))
            {
                var existeTipo = await _unitOfWork.TipoProductoRepository.ObtenerTipoProductoPorId(command.ProductoRequisito.IdTipoProducto);
                if (existeTipo.Value is null)
                {
                    return ErroresProductoRequisito.TipoProductoNoEncontrada;
                }

                comprobarIdTipoProducto = command.ProductoRequisito.IdTipoProducto;
                comprobarDuplicado = true;
            }

            if (command.ListaCambios.Contains("IdRequisito"))
            {
                var existeRequisito = await _unitOfWork.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(command.ProductoRequisito.IdRequisito);
                if (existeRequisito.Value is null)
                {
                    return ErroresProductoRequisito.RequisitoNoEncontrado;
                }

                comprobarIdRequisito = command.ProductoRequisito.IdRequisito;
                comprobarDuplicado = true;
            }

            //se obtiene copia de la productoRequisito antes de aplicar cambios
            Dominio.Entidades.ProductoRequisito productoRequisitoAntes = JsonConvert.DeserializeObject<Dominio.Entidades.ProductoRequisito>(
                JsonConvert.SerializeObject(productoRequisito.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? productoRequisito.Value;

            if (comprobarDuplicado)
            {
                var productoRequisitoExistente = await _unitOfWork.ProductoRequisitoRepository.ValidarProductoRequisitos(command.IdProductoRequisito, comprobarIdTipoProducto, comprobarIdRequisito);
                if (productoRequisitoExistente.Value)
                {
                    return ErroresProductoRequisito.ProductoRequisitoDuplicados;
                }
            }

            var result = await _unitOfWork.ProductoRequisitoRepository.ActualizarProductoRequisito(command.IdProductoRequisito, command.ListaCambios, command.ProductoRequisito);
            if (result.IsError)
            {
                return result.Errors;
            }
            await _unitOfWork.Save();

            return Tuple.Create(productoRequisitoAntes, productoRequisito.Value);
        }
    }
}