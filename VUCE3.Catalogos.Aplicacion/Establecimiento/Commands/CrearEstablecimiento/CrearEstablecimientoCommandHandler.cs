using ErrorOr;
using MediatR;
using System.Text;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.CrearEstablecimiento
{
    public class CrearEstablecimientoCommandHandler : IRequestHandler<CrearEstablecimientoCommand, ErrorOr<Dominio.Entidades.Establecimiento>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearEstablecimientoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Establecimiento>> Handle(CrearEstablecimientoCommand request, CancellationToken cancellationToken)
        {
            //validar numeroCvo
            if (Validadores.LongitudMaximaNoNull(request.Establecimiento.NumeroCvo, 15))
            {
                return ErroresEstablecimiento.NumeroCvoInvalido;
            }

            //Validar NombreEstablecimiento
            if(Validadores.LongitudMaximaNoNull(request.Establecimiento.NombreEstablecimiento, 50))
            {
                return ErroresEstablecimiento.NombreEstablecimientoInvalido;
            }

            //validar actividadPrimaria
            if (Validadores.LongitudMaximaNoNull(request.Establecimiento.ActividadPrimaria, 50))
            {
                return ErroresEstablecimiento.ActividadPrimariaInvalido;
            }
            if (request.Establecimiento.ActividadSecundaria is not null && Validadores.LongitudMaxima(request.Establecimiento.ActividadSecundaria, 50))
            {
                    return ErroresEstablecimiento.ActividadSecundariaInvalido;
            }
            //validar provincia
            if (Validadores.LongitudMaximaNoNull(request.Establecimiento.Provincia, 50))
            {
                return ErroresEstablecimiento.ProvinciaInvalido;
            }

            //validar canton
            if (Validadores.LongitudMaximaNoNull(request.Establecimiento.Canton, 50))
            {
                return ErroresEstablecimiento.CantonInvalido;
            }

            //Validar Distrito
            if (Validadores.LongitudMaximaNoNull(request.Establecimiento.Distrito, 50))
            {
                return ErroresEstablecimiento.DistritoInvalido;
            }

            //Validar DireccionExacta
            if (Validadores.LongitudMaximaNoNull(request.Establecimiento.DireccionExacta, 250))
            {
                return ErroresEstablecimiento.DireccionExactaInvalido;
            }

            // vaslidar fechaVencimiento
            if (request.Establecimiento.FechaVencimiento.Date <= DateTime.Today)
            {
                return ErroresEstablecimiento.FechaVencimientoMenorHoy;
            }
            //Validar EstadoEstablecimiento
            if (Validadores.LongitudMaximaNoNull(request.Establecimiento.EstadoEstablecimiento, 20))
            {
                return ErroresEstablecimiento.EstadoEstablecimientoInvalido;
            } 

            //Valida si existe establecimiento
            var existe = await _unitOfWork.EstablecimientosRepository.ValidarEstablecimiento(request.Establecimiento.Id, request.Establecimiento.NumeroCvo);
            if (existe.Value)
            {
                return ErroresEstablecimiento.EstablecimientoExiste;
            }

            var creado = await _unitOfWork.EstablecimientosRepository.CrearEstablecimiento(request.Establecimiento);

            if(creado.IsError)
            {
                return creado.Errors;
            }

            await _unitOfWork.Save();

            return creado;
        }
    }
}
