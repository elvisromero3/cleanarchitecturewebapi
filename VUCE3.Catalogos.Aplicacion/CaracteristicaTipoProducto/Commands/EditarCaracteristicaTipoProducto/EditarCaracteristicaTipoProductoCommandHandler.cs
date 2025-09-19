using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EditarCaracteristicaTipoProducto;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EditarCaracteristicaTipoProducto
{
    public class EditarCaracteristicaTipoProductoCommandHandler : IRequestHandler<EditarCaracteristicaTipoProductoCommand, ErrorOr<Tuple<Dominio.Entidades.CaracteristicaTipoProducto, Dominio.Entidades.CaracteristicaTipoProducto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarCaracteristicaTipoProductoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.CaracteristicaTipoProducto, Dominio.Entidades.CaracteristicaTipoProducto>>> Handle(EditarCaracteristicaTipoProductoCommand command, CancellationToken cancellationToken)
        {
            // Obtener CaracteristicaTipoProducto por Id
            var familia = await _unitOfWork.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductoPorId(command.IdCaracteristicaTipoProducto);
            if (familia.IsError)
            {
                return familia.Errors;
            }
           
            //se obtiene copia de la familia antes de aplicar cambios
            Dominio.Entidades.CaracteristicaTipoProducto familiaAntes = JsonConvert.DeserializeObject<Dominio.Entidades.CaracteristicaTipoProducto>(
                JsonConvert.SerializeObject(familia.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? familia.Value;

            var existe = await _unitOfWork.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(command.CaracteristicaTipoProducto.IdTipoProducto,command.CaracteristicaTipoProducto.IdCaracteristica);
            if (existe.Value)
            {
                return ErroresCaracteristicaTipoProducto.DatosDuplicados;
            }


            var result = await _unitOfWork.CaracteristicaTipoProductoRepository.ActualizarCaracteristicaTipoProducto(command.IdCaracteristicaTipoProducto, command.ListaCambios, command.CaracteristicaTipoProducto);
            if (result.IsError)
            {
                return result.Errors;
            }
            await _unitOfWork.Save();

            return Tuple.Create(familiaAntes, familia.Value);
        }
    }
}