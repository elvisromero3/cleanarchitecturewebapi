using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Requisitos.Command.EditarRequisito
{
    public class EditarRequisitoCommandHandler : IRequestHandler<EditarRequisitoCommand, ErrorOr<Tuple<Requisito, Requisito>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccesoGestionUsuariosService _accesoUsuariosService;

        public EditarRequisitoCommandHandler(IUnitOfWork unitOfWork, IAccesoGestionUsuariosService accesoUsuariosService)
        {
            _unitOfWork = unitOfWork;
            _accesoUsuariosService = accesoUsuariosService;
        }

        public async Task<ErrorOr<Tuple<Requisito, Requisito>>> Handle(EditarRequisitoCommand command, CancellationToken cancellationToken)
        {
            // Obtener requisito por Id
            var requisito = await _unitOfWork.RequisitosRepository.ObtenerRequisitoPorId(command.IdRequisito);
            if (requisito.IsError)
            {
                return requisito.Errors;
            }

            var comprobarCodigo = requisito.Value.Codigo;
            var comprobarVersion = requisito.Value.Version;
            var comprobarIdPais = requisito.Value.IdPais;
            var comprobarIdInstitucion = requisito.Value.IdInstitucion;
            var comprobarDuplicado = false;

            //Valida Codigo
            if (command.ListaCambios.Contains("Codigo"))
            {
                if (Validadores.LongitudMaximaNoNull(command.Requisito.Codigo, 50))
                {
                    return ErroresRequisito.RequisitoCodigoTamano;
                }

                comprobarCodigo = command.Requisito.Codigo;
                comprobarDuplicado = true;
            }

            //Valida tamaño versión
            if (command.ListaCambios.Contains("Version"))
            {
                if (Validadores.LongitudMaximaNoNull(command.Requisito.Version, 10))
                {
                    return ErroresRequisito.RequisitoDescripcionInvalida;
                }

                comprobarVersion = command.Requisito.Version;
                comprobarDuplicado = true;
            }

            //Valida tamaño descipción
            if (command.ListaCambios.Contains("Descripcion") && Validadores.LongitudMaximaNoNull(command.Requisito.Descripcion, 300))
            {
                return ErroresRequisito.RequisitoDescripcionInvalida;
            }

            if (command.ListaCambios.Contains("IdPais"))
            {
                var pais = await _unitOfWork.PaisesRepository.ObtenerPaisPorId(command.Requisito.IdPais);
                if (pais.IsError)
                {
                    return pais.Errors;
                }

                comprobarIdPais = command.Requisito.IdPais;
                comprobarDuplicado = true;
            }

            //se verifica que s ehaya modificado la institucion
            if (command.ListaCambios.Contains("IdInstitucion") && requisito.Value.IdInstitucion != command.Requisito.IdInstitucion)
            {
                var instituciones = await _accesoUsuariosService.ObtenerInstituciones();
                if (instituciones.IsError)
                {
                    return ErroresRequisito.RequisitoFalloServicioAccesoGestionUsuario;
                }
                var institucion = instituciones.Value.Find(x => x.Id == command.Requisito.IdInstitucion);
                if (institucion is null)
                {
                    return ErroresRequisito.RequisitoInstitucionNoEncontrada;
                }

                if (Validadores.institucionDCAoDIPOA(command.Requisito.IdInstitucion))
                {
                    return ErroresRequisito.RequisitoInstitucionInvalida;
                }

                var listaproductosRequisitos = await _unitOfWork.ProductoRequisitoRepository.ObtenerProductoRequisitos();
                if (!listaproductosRequisitos.IsError)
                {
                    var productosRequisitos = listaproductosRequisitos.Value;
                    if (productosRequisitos.Any(pr => pr.IdRequisito == command.IdRequisito))
                        return ErroresRequisito.RequisitoTieneProductosAsociados;
                }

                comprobarIdInstitucion = command.Requisito.IdInstitucion;
                comprobarDuplicado = true;
            }

            //Valida existencia de imagen
            if (command.ListaCambios.Contains("ImagenRequisito") && string.IsNullOrWhiteSpace(command.Requisito.ImagenRequisito))
            {
                return ErroresRequisito.RequisitoImagenObligatoria;
            }

            //Valida tamaño nombre imagen
            if (command.ListaCambios.Contains("NombreImagenRequisito") && string.IsNullOrWhiteSpace(command.Requisito.NombreImagenRequisito))
            {
                return ErroresRequisito.RequisitoNombreImagen;
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.RequisitosRepository.ValidarRequisito(command.IdRequisito, comprobarCodigo, comprobarVersion, comprobarIdPais, comprobarIdInstitucion);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresRequisito.RequisitoDatosDuplicados;
                }
            }

            //se obtiene copia de la requisito antes de aplicar cambios
            Requisito requisitoAntes = JsonConvert.DeserializeObject<Requisito>(
                JsonConvert.SerializeObject(requisito.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? requisito.Value;

            var result = await _unitOfWork.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(requisitoAntes, requisito.Value);
        }
    }
}
