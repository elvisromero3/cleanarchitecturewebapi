using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.EditarTipoProducto
{
    public class EditarTipoProductoCommandHandler : IRequestHandler<EditarTipoProductoCommand, ErrorOr<Tuple<Dominio.Entidades.TipoProducto, Dominio.Entidades.TipoProducto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarTipoProductoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.TipoProducto, Dominio.Entidades.TipoProducto>>> Handle(EditarTipoProductoCommand command, CancellationToken cancellationToken)
        {
            // Obtener tipoProducto por Id
            var tipoProducto = await _unitOfWork.TipoProductoRepository.ObtenerTipoProductoPorId(command.IdTipoProducto);
            if (tipoProducto.IsError)
            {
                return tipoProducto.Errors;
            }

            var comprobarTipo = tipoProducto.Value.Tipo;
            var comprobarInstitucion = tipoProducto.Value.IdInstitucion;
            var comprobarCategoria = tipoProducto.Value.IdCategoria;
            var comprobarDuplicado = false;

            if (command.ListaCambios.Contains("IdInstitucion") && tipoProducto.Value.IdInstitucion!=command.TipoProducto.IdInstitucion )
            {
                if (command.TipoProducto.IdInstitucion != ConstantesInstituciones.DCA && command.TipoProducto.IdInstitucion != ConstantesInstituciones.DIPOA)
                {
                    return ErroresTipoProducto.InstitucionNoValida;
                }

                var existeCaracteristicaTipoProducto = await _unitOfWork.CaracteristicaTipoProductoRepository.VerificaExistenciaPorTipoProducto(command.IdTipoProducto);
                if (existeCaracteristicaTipoProducto.Value)
                {
                    return ErroresTipoProducto.CaracteristicaRelacionadas;
                }

                comprobarInstitucion = command.TipoProducto.IdInstitucion;
                comprobarDuplicado = true;
            }

            if (command.ListaCambios.Contains("IdCategoria") && tipoProducto.Value.IdCategoria != command.TipoProducto.IdCategoria)
            {
                comprobarCategoria = command.TipoProducto.IdCategoria;
                comprobarDuplicado = true;
            }

            if (command.ListaCambios.Contains("Tipo"))
            {
                if (Validadores.LongitudMaximaNoNull(command.TipoProducto.Tipo, 200))
                {
                    return ErroresTipoProducto.TipoTamano;
                }
                comprobarTipo = command.TipoProducto.Tipo;
                comprobarDuplicado = true;
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.TipoProductoRepository.ValidarTipoProducto(comprobarTipo, comprobarCategoria, comprobarInstitucion, command.IdTipoProducto);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresTipoProducto.DatosDuplicados;
                }
            }

            //se obtiene copia de la tipoProducto antes de aplicar cambios
            Dominio.Entidades.TipoProducto tipoProductoAntes = JsonConvert.DeserializeObject<Dominio.Entidades.TipoProducto>(
                JsonConvert.SerializeObject(tipoProducto.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? tipoProducto.Value;

            var result = await _unitOfWork.TipoProductoRepository.ActualizarTipoProducto(command.IdTipoProducto, command.ListaCambios, command.TipoProducto);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(tipoProductoAntes, tipoProducto.Value);
        }
    }
}
