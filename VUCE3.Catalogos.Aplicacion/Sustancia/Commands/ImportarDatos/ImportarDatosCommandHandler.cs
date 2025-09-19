using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Sustancia.Commands.ImportarDatos
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
                return ErroresSustancia.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var resultadoDelete = await EliminarSustancias();
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            foreach (var datoInsertar in request.Datos)
            {
                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.SustanciasRepository.ValidarSustancia(0, datoInsertar.Nombre, datoInsertar.Cas, datoInsertar.ListaCaq);

                    if (existe.Value)
                    {
                        return ErroresSustancia.DatosDuplicados;
                    }
                }

                if (Validadores.LongitudMaximaNoNull(datoInsertar.Nombre, 300))
                {
                    return ErroresSustancia.SustanciaNombreInvalido;
                }

                if (Validadores.LongitudMaximaNoNull(datoInsertar.Cas, 100))
                {
                    return ErroresSustancia.SustanciasCasInvalido;
                }

                if (Validadores.LongitudMaximaNoNull(datoInsertar.ListaCaq, 100))
                {
                    return ErroresSustancia.SustanciaListaCaqInvalido;
                }


                var resultInsertar = await _unitOfWork.SustanciasRepository.CrearSustancia(datoInsertar);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private async Task<ErrorOr<Success>> EliminarSustancias()
        {
            var datosExistentes = await _unitOfWork.SustanciasRepository.ObtenerSustancias();

            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            foreach (var datoExistente in datosExistentes.Value)
            {
                var resultadoDelete = await _unitOfWork.SustanciasRepository.EliminarSustancia(datoExistente.Id);

                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Success;
        }

        private static bool ExistenDuplicados(IEnumerable<Dominio.Entidades.Sustancia> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var sustancia in datos)
            {                
                if (!datosComprobados.Add(Validadores.NormalizarString(sustancia.Nombre) + "_" + Validadores.NormalizarString(sustancia.Cas) + "_" + Validadores.NormalizarString(sustancia.ListaCaq)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
