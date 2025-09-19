using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Requisitos.Command.CrearRequisito
{
    public class CrearRequisitoCommandHandler : IRequestHandler<CrearRequisitoCommand, ErrorOr<Requisito>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccesoGestionUsuariosService _accesoUsuariosService;
        public CrearRequisitoCommandHandler(IUnitOfWork unitOfWork, IAccesoGestionUsuariosService accesoUsuariosService)
        {
            _unitOfWork = unitOfWork;
            _accesoUsuariosService = accesoUsuariosService;
        }
        public async Task<ErrorOr<Requisito>> Handle(CrearRequisitoCommand command, CancellationToken cancellationToken)
        {
            //Verifica tamaño de descripción o si es vacío 
            if (Validadores.LongitudMaximaNoNull(command.Requisito.Descripcion, 300))
            {
                return ErroresRequisito.RequisitoDescripcionInvalida;
            }
            //Verifica tamaño de Codigo
            if (Validadores.LongitudMaximaNoNull(command.Requisito.Codigo, 50))
            {
                return ErroresRequisito.RequisitoCodigoTamano;
            }

            //Verifica tamaño de Version
            if (Validadores.LongitudMaximaNoNull(command.Requisito.Version, 10))
            {
                return ErroresRequisito.RequisitoVersionTamano;
            }

            //Verifica existencia de imagen  
            if (string.IsNullOrWhiteSpace(command.Requisito.ImagenRequisito))
            {
                return ErroresRequisito.RequisitoImagenObligatoria;
            }

            //Verifica tamaño nombre de imagen  
            if (string.IsNullOrWhiteSpace(command.Requisito.NombreImagenRequisito))
            {
                return ErroresRequisito.RequisitoNombreImagen;
            }

            var pais = await _unitOfWork.PaisesRepository.ObtenerPaisPorId(command.Requisito.IdPais);
            if (pais.IsError)
            {
                return pais.Errors;
            }

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
            //Verifica la institucion
            if (Validadores.institucionDCAoDIPOA(command.Requisito.IdInstitucion))
            {
                return ErroresRequisito.RequisitoInstitucionInvalida;
            }

            var existe = await _unitOfWork.RequisitosRepository.ValidarRequisito(command.Requisito.Id, command.Requisito.Codigo, command.Requisito.Version, command.Requisito.IdPais, command.Requisito.IdInstitucion);
            if (existe.Value)
            {
                return ErroresRequisito.RequisitoDatosDuplicados;
            }

            command.Requisito.Activo = true;

            var result = await _unitOfWork.RequisitosRepository.CrearRequisito(command.Requisito);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
