using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EditarExcepcionMorosidad
{
    public class EditarExcepcionMorosidadCommandHandler : IRequestHandler<EditarExcepcionMorosidadCommand, ErrorOr<Tuple<ExcepcionMorosidad, ExcepcionMorosidad>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarExcepcionMorosidadCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<ExcepcionMorosidad, ExcepcionMorosidad>>> Handle(EditarExcepcionMorosidadCommand command, CancellationToken cancellationToken)
        {

            var excepcion = await _unitOfWork.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(command.IdExcepcionMorosidad);
            if (excepcion.IsError)
            {
                return excepcion.Errors;
            }

            var comprobarIdTipoIdentificacion = excepcion.Value.TipoIdentificacionEmpresa;
            var comprobarNumeroIdentificacion = excepcion.Value.NumeroIdentificacionEmpresa;
            var comprobarIdTipoTramite = excepcion.Value.IdTipoTramite;
            var comprobarIdSubtipoTramite = excepcion.Value.IdSubtipoTramite;
            var comprobarIdTipoAccion = excepcion.Value.IdTipoAccion;
            var comprobarIdRegimen = excepcion.Value.IdRegimen;
            var comprobarFechaInicio = excepcion.Value.FechaInicio;
            var comprobarFechaVencimiento = excepcion.Value.FechaVencimiento;

            ExcepcionMorosidad excepcionAntes = JsonConvert.DeserializeObject<ExcepcionMorosidad>(
                JsonConvert.SerializeObject(excepcion.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? excepcion.Value;

            if (command.ListaCambios.Contains("TipoIdentificacionEmpresa"))
            {
                if (command.ExcepcionMorosidad.TipoIdentificacionEmpresa != ConstantesIdTipoIdentificacion.FISICA &&
                command.ExcepcionMorosidad.TipoIdentificacionEmpresa != ConstantesIdTipoIdentificacion.JURIDICA &&
                command.ExcepcionMorosidad.TipoIdentificacionEmpresa != ConstantesIdTipoIdentificacion.DIMEX &&
                command.ExcepcionMorosidad.TipoIdentificacionEmpresa != ConstantesIdTipoIdentificacion.PASAPORTE)
                {
                    return ErroresExcepcionMorosidad.TipoIdentificacionNoEncontrado;
                }

                comprobarIdTipoIdentificacion = command.ExcepcionMorosidad.TipoIdentificacionEmpresa;
            }

            if (command.ListaCambios.Contains("NumeroIdentificacionEmpresa"))
            {
                comprobarNumeroIdentificacion = command.ExcepcionMorosidad.NumeroIdentificacionEmpresa;
            }

            if (command.ListaCambios.Contains("NumeroIdentificacionEmpresa") || command.ListaCambios.Contains("TipoIdentificacionEmpresa"))
            {
                if (Validadores.NumeroIdentificacion(comprobarIdTipoIdentificacion, comprobarNumeroIdentificacion))
                {
                    return ErroresExcepcionMorosidad.NumeroIdentificacionExcedeLimite;
                }

                //se valida identificación física comienza con 0
                if (Validadores.TipoIdentificacionFisicaComienza0(comprobarIdTipoIdentificacion, comprobarNumeroIdentificacion))
                {
                    return ErroresExcepcionMorosidad.TipoIdentificacionFisicaComienza0;
                }
            }           

            if (command.ListaCambios.Contains("FechaVencimiento"))
            {
                comprobarFechaVencimiento = command.ExcepcionMorosidad.FechaVencimiento;
            }

            if (command.ListaCambios.Contains("FechaInicio"))
            {
                comprobarFechaInicio = command.ExcepcionMorosidad.FechaInicio;
            }

            if (command.ListaCambios.Contains("FechaInicio") || command.ListaCambios.Contains("FechaFin"))
            {

                // La fecha de fin no puede ser menor a la fecha de inicio
                if (comprobarFechaInicio >= comprobarFechaVencimiento)
                {
                    return ErroresExcepcionMorosidad.FechaInicioMayorIgualFechaVencimiento;
                }

                // La fecha de inicio no puede ser menor a hoy
                if (comprobarFechaInicio.Date < DateTime.Now.Date)
                {
                    return ErroresExcepcionMorosidad.FechaInicioMenorAlDia;
                }

                // Si la fecha de inicio ya ha pasado, el estado se guardará como "Activa"
                if (comprobarFechaInicio.Date <= DateTime.Now.Date)
                {
                    command.ExcepcionMorosidad.IdEstado = ConstantesEstadosExcepcionMorosidad.ACTIVA;
                }
                else
                {
                    command.ExcepcionMorosidad.IdEstado = ConstantesEstadosExcepcionMorosidad.PROGRAMADA;
                }
            }

            if (command.ListaCambios.Contains("IdTipoTramite"))
            {
                comprobarIdTipoTramite = command.ExcepcionMorosidad.IdTipoTramite;
            }
            if (command.ListaCambios.Contains("IdSubtipoTramite"))
            {
                comprobarIdSubtipoTramite = command.ExcepcionMorosidad.IdSubtipoTramite;
            }
            if (command.ListaCambios.Contains("IdTipoAccion"))
            {
                comprobarIdTipoAccion = command.ExcepcionMorosidad.IdTipoAccion;
            }
            if (command.ListaCambios.Contains("IdRegimen"))
            {
                comprobarIdRegimen = command.ExcepcionMorosidad.IdRegimen;
            }

            if (command.ListaCambios.Contains("IdTipoTramite") && command.ExcepcionMorosidad.IdTipoTramite == ConstantesTipoTramite.TipoTramite_NOTATECNICA && command.ExcepcionMorosidad.IdRegimen == null)
            {
                return ErroresExcepcionMorosidad.TipoTramiteNTRegimen;
            }

            if (command.ListaCambios.Contains("IdTipoTramite") && command.ExcepcionMorosidad.IdTipoTramite != ConstantesTipoTramite.TipoTramite_NOTATECNICA && command.ExcepcionMorosidad.IdRegimen > 0)
            {
                return ErroresExcepcionMorosidad.TipoTramiteRegimen;
            }

            //validar que no existe un registro duplicado
            var excepcionMorosidadComprobarDuplicidad = new ExcepcionMorosidad()
            {
                Id = command.IdExcepcionMorosidad,
                IdTipoTramite = comprobarIdTipoTramite,
                IdSubtipoTramite = comprobarIdSubtipoTramite,
                IdRegimen = comprobarIdRegimen,
                IdTipoAccion = comprobarIdTipoAccion,
                NumeroIdentificacionEmpresa = comprobarNumeroIdentificacion,
                FechaInicio = comprobarFechaInicio,
                FechaVencimiento = comprobarFechaVencimiento
            };

            var existeExcepcionMorosidad = await _unitOfWork.ExcepcionesMorosidadRepository.ValidarExcepcionMorosidad(excepcionMorosidadComprobarDuplicidad, false);
            if (existeExcepcionMorosidad.Value)
            {
                return ErroresExcepcionMorosidad.ExcepcionMorosidadDatosDuplicados;
            }

            var result = await _unitOfWork.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(command.IdExcepcionMorosidad, command.ListaCambios, command.ExcepcionMorosidad);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(excepcionAntes, excepcion.Value);
        }
    }
}
