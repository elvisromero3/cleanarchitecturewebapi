using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Cantones.Commands.ImportarDatos
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
                return ErroresCanton.DatosDuplicadosArchivo;
            }

            var provincias = await _unitOfWork.ProvinciaRepository.ObtenerProvincias();

            foreach (var datoInsertar in request.Datos)
            {
                if (!provincias.Value.Any(p => p.Id == datoInsertar.Provincia))
                {
                    return ErroresCanton.ProvinciaInexistente;
                }

                if (Validadores.Codigo3(datoInsertar.Codigo))
                {
                    return ErroresCanton.CantonCodigoInvalido;
                }

                if (Validadores.Nombre50(datoInsertar.Nombre))
                {
                    return ErroresCanton.NombreInvalido;
                }

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.CantonesRepository.ValidarCantones(0, datoInsertar.Codigo, 0);
                    if (existe.Value)
                    {
                        return ErroresCanton.DatosDuplicados;
                    }
                }
                else // REEMPLAZAR EXISTENTES
                {
                    var existe = await _unitOfWork.CantonesRepository.ValidarCantones(0, datoInsertar.Codigo, datoInsertar.Provincia);
                    if (existe.Value)
                    {
                        return ErroresCanton.DatosDuplicados;
                    }

                    var resultadoDelete = await EliminarCantones(datoInsertar.Provincia);
                    if (resultadoDelete.IsError)
                    {
                        return resultadoDelete.Errors;
                    }
                }
                
                var cantonInsertar = new Canton
                {
                    Codigo = datoInsertar.Codigo,
                    Nombre = datoInsertar.Nombre,
                    IdProvincia = datoInsertar.Provincia
                };

                var resultInsertar = await _unitOfWork.CantonesRepository.CrearCantones(cantonInsertar);
                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }
            await _unitOfWork.Save();

            return Result.Created;
        }

        private async Task<ErrorOr<Success>> EliminarCantones(int idProvincia)
        {
            var datosExistentes = await _unitOfWork.CantonesRepository.ObtenerCantones();
            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            var cantonesFiltrados = datosExistentes.Value
            .Where(b => b.IdProvincia == idProvincia)
            .ToList();


            foreach (var datoExistente in cantonesFiltrados)
            {
                var resultadoDelete = await _unitOfWork.CantonesRepository.EliminarCantones(datoExistente.Id);
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            await _unitOfWork.Save();
            return Result.Success;
        }

        private static bool ExistenDuplicados(IEnumerable<ImportarCantonCommandDto> datos)
        {
            HashSet<string> datosComprobadosCodigo = new HashSet<string>();
            foreach (var canton in datos)
            {
                if (!datosComprobadosCodigo.Add(canton.Codigo))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
