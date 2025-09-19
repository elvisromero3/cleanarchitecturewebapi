using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.ImportarDatos
{
    public class ImportarDatosCommandHandler : IRequestHandler<ImportarDatosCommand, ErrorOr<Created>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITramitesService _tramitesService;

        public ImportarDatosCommandHandler(IUnitOfWork unitOfWork, ITramitesService tramitesService)
        {
            _unitOfWork = unitOfWork;
            _tramitesService = tramitesService;
        }
        public async Task<ErrorOr<Created>> Handle(ImportarDatosCommand request, CancellationToken cancellationToken)
        {
            //validar excepciones de morosidad duplicados 
            var excepcionMorosidadDuplicadas = request.Datos
                .SelectMany((actual, i) => request.Datos
                .Skip(i + 1)
                .Where(otro =>
                    actual.IdTipoTramite == otro.IdTipoTramite &&
                    actual.IdSubtipoTramite == otro.IdSubtipoTramite &&
                    actual.IdRegimen == otro.IdRegimen &&
                    actual.IdTipoAccion == otro.IdTipoAccion &&
                    actual.NumeroIdentificacionEmpresa == otro.NumeroIdentificacionEmpresa &&
                    actual.FechaInicio <= otro.FechaVencimiento &&
                    actual.FechaVencimiento >= otro.FechaInicio
                ))
             .Any();

            if (excepcionMorosidadDuplicadas)
            {
                return ErroresExcepcionMorosidad.ExcepcionMorosidadDatosDuplicados;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var datosExistentes = await _unitOfWork.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad();

                if (datosExistentes.IsError)
                {
                    return datosExistentes.Errors;
                }

                foreach (var datoExistente in datosExistentes.Value)
                {
                    if (datoExistente.IdEstado == ConstantesEstadosExcepcionMorosidad.PROGRAMADA)
                    {
                        var resultadoDelete = await _unitOfWork.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(datoExistente.Id);

                        if (resultadoDelete.IsError)
                        {
                            return resultadoDelete.Errors;
                        }
                    }

                }
            }

            foreach (var datoInsertar in request.Datos)
            {
                if (datoInsertar.TipoIdentificacionEmpresa != ConstantesIdTipoIdentificacion.FISICA &&
                    datoInsertar.TipoIdentificacionEmpresa != ConstantesIdTipoIdentificacion.JURIDICA &&
                    datoInsertar.TipoIdentificacionEmpresa != ConstantesIdTipoIdentificacion.DIMEX &&
                    datoInsertar.TipoIdentificacionEmpresa != ConstantesIdTipoIdentificacion.PASAPORTE)
                {
                    return ErroresExcepcionMorosidad.TipoIdentificacionNoEncontrado;
                }

                if (datoInsertar.IdTipoTramite <= 0)
                {
                    return ErroresExcepcionMorosidad.TipoTramiteInvalido;
                }

                if (datoInsertar.IdTipoAccion <= 0)
                {
                    return ErroresExcepcionMorosidad.DatosArchivoImportarInvalido;
                }

                if (datoInsertar.IdSubtipoTramite <= 0)
                {
                    return ErroresExcepcionMorosidad.DatosArchivoImportarInvalido;
                }

                //tipo tramite registro corresponde a un sub tipo de tramite
                var validTipoTramite = await _tramitesService.ExisteRelacionTipoTramiteSubtipo(datoInsertar.IdTipoTramite, datoInsertar.IdSubtipoTramite);
                if (validTipoTramite.IsError)
                {
                    return validTipoTramite.Errors;
                }

                if (!validTipoTramite.Value)
                {
                    return ErroresExcepcionMorosidad.DatosArchivoImportarInvalido;
                }

                //tipo de tramite registro no puede tener regimen
                if (datoInsertar.IdTipoTramite == ConstantesTipoTramite.TipoTramite_NOTATECNICA && datoInsertar.IdRegimen == null)
                {
                    return ErroresExcepcionMorosidad.DatosArchivoImportarInvalido;
                }

                if (datoInsertar.IdTipoTramite == ConstantesTipoTramite.TipoTramite_NOTATECNICA && (datoInsertar.IdRegimen <= 0 || datoInsertar.IdRegimen == null))
                {
                    return ErroresExcepcionMorosidad.TipoTramiteNTRegimen;
                }

                if (datoInsertar.IdTipoTramite != ConstantesTipoTramite.TipoTramite_NOTATECNICA && (datoInsertar.IdRegimen > 0 || datoInsertar.IdRegimen == -1) )
                {
                    return ErroresExcepcionMorosidad.TipoTramiteRegimen;
                }

                if (Validadores.NumeroIdentificacion(datoInsertar.TipoIdentificacionEmpresa, datoInsertar.NumeroIdentificacionEmpresa))
                {
                    return ErroresExcepcionMorosidad.NumeroIdentificacionExcedeLimite;
                }

                if (Validadores.TipoIdentificacionFisicaComienza0(datoInsertar.TipoIdentificacionEmpresa, datoInsertar.NumeroIdentificacionEmpresa))
                {
                    return ErroresExcepcionMorosidad.TipoIdentificacionFisicaComienza0;
                }

                if (datoInsertar.FechaInicio >= datoInsertar.FechaVencimiento)
                {
                    return ErroresExcepcionMorosidad.FechaInicioMayorIgualFechaVencimiento;
                }

                if (datoInsertar.FechaInicio.Date < DateTime.Now.Date)
                {
                    return ErroresExcepcionMorosidad.FechaInicioMenorAlDia;
                }

                if (String.IsNullOrWhiteSpace(datoInsertar.NombreEmpresa) || datoInsertar.NombreEmpresa.Length > 100)
                {
                    return ErroresExcepcionMorosidad.NombreEmpresaInvalido;
                }

                if (String.IsNullOrWhiteSpace(datoInsertar.Observaciones) || datoInsertar.Observaciones.Length > 250)
                {
                    return ErroresExcepcionMorosidad.ObservacionesInvalidas;
                }

                // Si la fecha de inicio ya ha pasado, el estado se guardará como "Activa"
                if (datoInsertar.FechaInicio.Date <= DateTime.Now.Date)
                {
                    datoInsertar.IdEstado = ConstantesEstadosExcepcionMorosidad.ACTIVA;
                }
                // Si la fecha de inicio no ha llegado, el estado se guardará como "Programada"
                else
                {
                    datoInsertar.IdEstado = ConstantesEstadosExcepcionMorosidad.PROGRAMADA;
                }

                var excepcion = new Dominio.Entidades.ExcepcionMorosidad()
                {
                    IdTipoTramite = datoInsertar.IdTipoTramite,
                    IdSubtipoTramite = datoInsertar.IdSubtipoTramite,
                    IdRegimen = datoInsertar.IdRegimen,
                    IdTipoAccion = datoInsertar.IdTipoAccion,
                    NombreEmpresa = datoInsertar.NombreEmpresa,
                    Observaciones = datoInsertar.Observaciones,
                    TipoIdentificacionEmpresa = datoInsertar.TipoIdentificacionEmpresa,
                    NumeroIdentificacionEmpresa = datoInsertar.NumeroIdentificacionEmpresa,
                    FechaInicio = datoInsertar.FechaInicio,
                    FechaVencimiento = datoInsertar.FechaVencimiento,
                    IdEstado = datoInsertar.IdEstado
                };

                //validar que no existe un registro duplicado
                var validarExcepcionMorosidad = await _unitOfWork.ExcepcionesMorosidadRepository
                    .ValidarExcepcionMorosidad(excepcion, request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR);
                if (validarExcepcionMorosidad.Value)
                {
                    return ErroresExcepcionMorosidad.ExcepcionMorosidadDatosDuplicados;
                }

                var resultInsertar = await _unitOfWork.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }
    }
}
