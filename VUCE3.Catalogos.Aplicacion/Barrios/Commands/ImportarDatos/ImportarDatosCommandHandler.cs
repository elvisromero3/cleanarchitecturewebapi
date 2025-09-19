using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Commands.ImportarDatos
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
                return ErroresBarrio.DatosDuplicadosArchivo;
            }

            var provincias = await _unitOfWork.ProvinciaRepository.ObtenerProvincias();

            var cantones = await _unitOfWork.CantonesRepository.ObtenerCantones();

            var distritos = await _unitOfWork.DistritosRepository.ObtenerDistritos();

            foreach (var datoInsertar in request.Datos)
            {
                if (!provincias.Value.Any(p => p.Id == datoInsertar.Provincia))
                {
                    return ErroresDistrito.ProvinciaInexistente;
                }

                if (!cantones.Value.Any(p => p.Id == datoInsertar.Canton))
                {
                    return ErroresDistrito.CantonInexistente;
                }

                if (!distritos.Value.Any(p => p.Id == datoInsertar.Distrito))
                {
                    return ErroresBarrio.DistritoInexistente;
                }

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.BarriosRepository.ValidarBarrio(0, datoInsertar.Codigo, 0);
                    if (existe.Value)
                    {
                        return ErroresBarrio.BarrioDatosDuplicados;
                    }
                }
                else // REEMPLAZAR EXISTENTES
                {
                    var existe = await _unitOfWork.BarriosRepository.ValidarBarrio(0, datoInsertar.Codigo, datoInsertar.Distrito);
                    if (existe.Value)
                    {
                        return ErroresBarrio.BarrioDatosDuplicados;
                    }

                    var resultadoDelete = await EliminarBarrios(datoInsertar.Distrito);
                    if (resultadoDelete.IsError)
                    {
                        return resultadoDelete.Errors;
                    }
                }

                if (Validadores.Nombre100(datoInsertar.Nombre))
                {
                    return ErroresBarrio.BarrioNombreInvalido;
                }
                if (Validadores.Codigo7(datoInsertar.Codigo))
                {
                    return ErroresBarrio.BarrioCodigoInvalido;
                }

                var barrioInsertar = new Barrio
                {
                    Codigo = datoInsertar.Codigo,
                    Nombre = datoInsertar.Nombre,
                    IdDistrito = datoInsertar.Distrito
                };

                var resultInsertar = await _unitOfWork.BarriosRepository.CrearBarrio(barrioInsertar);
                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }
            await _unitOfWork.Save();

            return Result.Created;
        }

        private async Task<ErrorOr<Success>> EliminarBarrios(int idDistrito)
        {
            var datosExistentes = await _unitOfWork.BarriosRepository.ObtenerBarrios();
            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            var barriosFiltrados = datosExistentes.Value
                .Where(b => b.IdDistrito == idDistrito)
                .ToList();

            foreach (var datoExistente in barriosFiltrados)
            {
                var resultadoDelete = await _unitOfWork.BarriosRepository.EliminarBarrio(datoExistente.Id);
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }
            await _unitOfWork.Save();
            return Result.Success;
        }

        private static bool ExistenDuplicados(IEnumerable<ImportarBarrioCommandDto> datos)
        {
            HashSet<string> datosComprobadosCodigo = new HashSet<string>();
            foreach (var barrio in datos)
            {
                if (!datosComprobadosCodigo.Add(barrio.Codigo))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
