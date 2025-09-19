using ErrorOr;
using MediatR;
using System.Runtime.ConstrainedExecution;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.CrearExcepcionMorosidad
{
    public class CrearExcepcionMorosidadCommandHandler : IRequestHandler<CrearExcepcionMorosidadCommand, ErrorOr<ExcepcionMorosidad>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearExcepcionMorosidadCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<ExcepcionMorosidad>> Handle(CrearExcepcionMorosidadCommand request, CancellationToken cancellationToken)
        {
            if (request.ExcepcionMorosidad.TipoIdentificacionEmpresa != ConstantesIdTipoIdentificacion.FISICA &&
               request.ExcepcionMorosidad.TipoIdentificacionEmpresa != ConstantesIdTipoIdentificacion.JURIDICA &&
               request.ExcepcionMorosidad.TipoIdentificacionEmpresa != ConstantesIdTipoIdentificacion.DIMEX &&
               request.ExcepcionMorosidad.TipoIdentificacionEmpresa != ConstantesIdTipoIdentificacion.PASAPORTE)
            {
                return ErroresExcepcionMorosidad.TipoIdentificacionNoEncontrado;
            }

            if (Validadores.NumeroIdentificacion(request.ExcepcionMorosidad.TipoIdentificacionEmpresa, request.ExcepcionMorosidad.NumeroIdentificacionEmpresa))
            {
                return ErroresExcepcionMorosidad.NumeroIdentificacionExcedeLimite;
            }

            if (Validadores.TipoIdentificacionFisicaComienza0(request.ExcepcionMorosidad.TipoIdentificacionEmpresa, request.ExcepcionMorosidad.NumeroIdentificacionEmpresa))
            {
                return ErroresExcepcionMorosidad.TipoIdentificacionFisicaComienza0;
            }

            if (request.ExcepcionMorosidad.FechaInicio >= request.ExcepcionMorosidad.FechaVencimiento)
            {
                return ErroresExcepcionMorosidad.FechaInicioMayorIgualFechaVencimiento;
            }

            if (request.ExcepcionMorosidad.FechaInicio.Date < DateTime.Now.Date)
            {
                return ErroresExcepcionMorosidad.FechaInicioMenorAlDia;
            }

            if (Validadores.Nombre(request.ExcepcionMorosidad.NombreEmpresa))
            {
                return ErroresExcepcionMorosidad.NombreEmpresaInvalido;
            }

            if (Validadores.LongitudMaximaNoNull(request.ExcepcionMorosidad.Observaciones, 250))
            {
                return ErroresExcepcionMorosidad.ObservacionesInvalidas;
            }
            
            // Si la fecha de inicio ya ha pasado, el estado se guardará como "Activa"
            if (request.ExcepcionMorosidad.FechaInicio.Date <= DateTime.Now.Date)
            {
                request.ExcepcionMorosidad.IdEstado = ConstantesEstadosExcepcionMorosidad.ACTIVA;
            }// Si la fecha de inicio no ha llegado, el estado se guardará como "Programada"
            else
            {
                request.ExcepcionMorosidad.IdEstado = ConstantesEstadosExcepcionMorosidad.PROGRAMADA;
            }

            if (request.ExcepcionMorosidad.IdTipoTramite == ConstantesTipoTramite.TipoTramite_NOTATECNICA && request.ExcepcionMorosidad.IdRegimen == null)
            {
                return ErroresExcepcionMorosidad.TipoTramiteNTRegimen;
            }

            if (request.ExcepcionMorosidad.IdTipoTramite != ConstantesTipoTramite.TipoTramite_NOTATECNICA && request.ExcepcionMorosidad.IdRegimen > 0)
            {
                return ErroresExcepcionMorosidad.TipoTramiteRegimen;
            }

            var existeExcepcionMorosidad = await _unitOfWork.ExcepcionesMorosidadRepository.ValidarExcepcionMorosidad(request.ExcepcionMorosidad, false);
            if (existeExcepcionMorosidad.Value)
            {
                return ErroresExcepcionMorosidad.ExcepcionMorosidadDatosDuplicados;
            }

            var result = await _unitOfWork.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(request.ExcepcionMorosidad);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
