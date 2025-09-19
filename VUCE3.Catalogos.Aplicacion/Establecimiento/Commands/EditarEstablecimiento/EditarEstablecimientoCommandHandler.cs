using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EditarEstablecimiento
{
    public class EditarEstablecimientoCommandHandler : IRequestHandler<EditarEstablecimientoCommand, ErrorOr<Tuple<Dominio.Entidades.Establecimiento, Dominio.Entidades.Establecimiento>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarEstablecimientoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Tuple<Dominio.Entidades.Establecimiento, Dominio.Entidades.Establecimiento>>> Handle(EditarEstablecimientoCommand request, CancellationToken cancellationToken)
        {
            // Obtener establecimiento por Id
            var establecimiento = await _unitOfWork.EstablecimientosRepository.ObtenerEstablecimientoPorId(request.IdEstablecimiento);
            if (establecimiento.IsError)
            {
                return establecimiento.Errors;
            }

           
            //validar numeroCvo
            if (request.ListaCambios.Contains("NumeroCvo")) { 
                
                if( Validadores.LongitudMaximaNoNull(request.Establecimiento.NumeroCvo, 15))
                {
                    return ErroresEstablecimiento.NumeroCvoInvalido;
                }

                var existe = await _unitOfWork.EstablecimientosRepository.ValidarEstablecimiento(establecimiento.Value.Id, request.Establecimiento.NumeroCvo);
                if (existe.Value)
                {
                    return ErroresEstablecimiento.EstablecimientoExiste;
                }
            }
            //validar NombreEstablecimiento
            if (request.ListaCambios.Contains("NombreEstablecimiento") && Validadores.LongitudMaximaNoNull(request.Establecimiento.NombreEstablecimiento, 50))
            {
                return ErroresEstablecimiento.NombreEstablecimientoInvalido;
            }

            //validar actividadPrimaria
            if (request.ListaCambios.Contains("ActividadPrimaria") &&
                Validadores.LongitudMaximaNoNull(request.Establecimiento.ActividadPrimaria, 50))
            {
                return ErroresEstablecimiento.ActividadPrimariaInvalido;
            }

            //validar actividadSecundaria
            if (request.ListaCambios.Contains("ActividadSecundaria") &&
                request.Establecimiento.ActividadSecundaria is not null && 
                Validadores.LongitudMaxima(request.Establecimiento.ActividadSecundaria, 50))
            {
                return ErroresEstablecimiento.ActividadSecundariaInvalido;
            }

            //validar provincia
            if (request.ListaCambios.Contains("Provincia") && Validadores.LongitudMaximaNoNull(request.Establecimiento.Provincia, 50))
            {
                return ErroresEstablecimiento.ProvinciaInvalido;
            }

            //validar canton
            if (request.ListaCambios.Contains("Canton") && Validadores.LongitudMaximaNoNull(request.Establecimiento.Canton, 50))
            {
                return ErroresEstablecimiento.CantonInvalido;
            }

            //Validar Distrito
            if (request.ListaCambios.Contains("Distrito") && Validadores.LongitudMaximaNoNull(request.Establecimiento.Distrito, 50))
            {
                return ErroresEstablecimiento.DistritoInvalido;
            }

            //Validar DireccionExacta
            if (request.ListaCambios.Contains("DireccionExacta") && Validadores.LongitudMaximaNoNull(request.Establecimiento.DireccionExacta, 250))
            {
                return ErroresEstablecimiento.DireccionExactaInvalido;
            }

            // vaslidar fechaVencimiento
            if (request.ListaCambios.Contains("FechaVencimiento") && request.Establecimiento.FechaVencimiento.Date <= DateTime.Today)
            {
                return ErroresEstablecimiento.FechaVencimientoMenorHoy;
            }
            //Validar EstadoEstablecimiento
            if (request.ListaCambios.Contains("EstadoEstablecimiento") && Validadores.LongitudMaximaNoNull(request.Establecimiento.EstadoEstablecimiento, 20))
            {
                return ErroresEstablecimiento.EstadoEstablecimientoInvalido;
            }
            var result = await _unitOfWork.EstablecimientosRepository.ActualizarEstablecimiento(request.IdEstablecimiento, request.ListaCambios, request.Establecimiento);

            if(result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(request.Establecimiento, result.Value);
        }
    }
}
