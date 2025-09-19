using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.ImportarDatos
{
    public class ImportarDatosCommandHandler : IRequestHandler<ImportarDatosCommand, ErrorOr<Created>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ImportarDatosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Created>> Handle(ImportarDatosCommand request, CancellationToken cancellationToken)
        {
            if (ExistenDuplicados(request.Datos))
            {
                return ErroresEstablecimiento.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var resultadoDelete = await eliminarEstablecimientos();
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }

            }

            foreach (var datoInsertar in request.Datos)
            {
                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.EstablecimientosRepository.ValidarEstablecimiento(datoInsertar.Id, datoInsertar.NumeroCvo);
                    if (existe.Value)
                    {
                        return ErroresEstablecimiento.DatosDuplicados;
                    }
                }
              
                //validar numeroCvo
                if (Validadores.LongitudMaximaNoNull(datoInsertar.NumeroCvo, 15))
                {
                    return ErroresEstablecimiento.NumeroCvoInvalido;
                }

                //Validar NombreEstablecimiento
                if (Validadores.LongitudMaximaNoNull(datoInsertar.NombreEstablecimiento, 50))
                {
                    return ErroresEstablecimiento.NombreEstablecimientoInvalido;
                }

                //validar actividadPrimaria
                if (Validadores.LongitudMaximaNoNull(datoInsertar.ActividadPrimaria, 50))
                {
                    return ErroresEstablecimiento.ActividadPrimariaInvalido;
                }

                //validar actividadSecundaria
                if (string.IsNullOrWhiteSpace(datoInsertar.ActividadSecundaria))
                {
                    datoInsertar.ActividadSecundaria = null;
                }

                if (datoInsertar.ActividadSecundaria is not null && Validadores.LongitudMaximaNoNull(datoInsertar.ActividadSecundaria, 50))
                {
                    return ErroresEstablecimiento.ActividadSecundariaInvalido;
                }

                //validar provincia
                if (Validadores.LongitudMaximaNoNull(datoInsertar.Provincia, 50))
                {
                    return ErroresEstablecimiento.ProvinciaInvalido;
                }

                //validar canton
                if (Validadores.LongitudMaximaNoNull(datoInsertar.Canton, 50))
                {
                    return ErroresEstablecimiento.CantonInvalido;
                }

                //Validar Distrito
                if (Validadores.LongitudMaximaNoNull(datoInsertar.Distrito, 50))
                {
                    return ErroresEstablecimiento.DistritoInvalido;
                }

                //Validar DireccionExacta
                if (Validadores.LongitudMaximaNoNull(datoInsertar.DireccionExacta, 250))
                {
                    return ErroresEstablecimiento.DireccionExactaInvalido;
                }

                // vaslidar fechaVencimiento
                if (datoInsertar.FechaVencimiento.Date <= DateTime.Today)
                {
                    return ErroresEstablecimiento.FechaVencimientoMenorHoy;
                }
                //Validar EstadoEstablecimiento
                if (Validadores.LongitudMaximaNoNull(datoInsertar.EstadoEstablecimiento, 20))
                {
                    return ErroresEstablecimiento.EstadoEstablecimientoInvalido;
                }
                var resultInsertar = await _unitOfWork.EstablecimientosRepository.CrearEstablecimiento(datoInsertar);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private async Task<ErrorOr<Success>> eliminarEstablecimientos()
        {
            var datosExistentes = await _unitOfWork.EstablecimientosRepository.ObtenerEstablecimientos();

            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            foreach (var datoExistente in datosExistentes.Value)
            {
                var resultadoDelete = await _unitOfWork.EstablecimientosRepository.EliminarEstablecimiento(datoExistente.Id);

                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Success;
        }

        private static bool ExistenDuplicados(IEnumerable<Dominio.Entidades.Establecimiento> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var establecimiento in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(establecimiento.NumeroCvo)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
