using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Commands.ImportarDatos
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
                return ErroresDistrito.DatosDuplicadosArchivo;
            }

            var provincias = await _unitOfWork.ProvinciaRepository.ObtenerProvincias();

            var cantones = await _unitOfWork.CantonesRepository.ObtenerCantones();

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

                if (Validadores.Codigo5(datoInsertar.Codigo))
                {
                    return ErroresDistrito.DistritoCodigoInvalido;
                }
                                
                if (Validadores.Nombre50(datoInsertar.Nombre))
                {
                    return ErroresDistrito.DistritoNombreInvalido;
                }

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.DistritosRepository.ValidarDistrito(0, datoInsertar.Codigo, 0);
                    if (existe.Value)
                    {
                        return ErroresDistrito.DistritoDatosDuplicados;
                    }
                }
                else // REEMPLAZAR EXISTENTES
                {

                    var existe = await _unitOfWork.DistritosRepository.ValidarDistrito(0, datoInsertar.Codigo, datoInsertar.Canton);
                    if (existe.Value)
                    {
                        return ErroresDistrito.DistritoDatosDuplicados;
                    }

                    var resultadoDelete = await EliminarDistritos(datoInsertar.Canton);
                    if (resultadoDelete.IsError)
                    {
                        return resultadoDelete.Errors;
                    }
                }

                var distritoInsertar = new Distrito
                {
                    Codigo = datoInsertar.Codigo,
                    Nombre = datoInsertar.Nombre,
                    IdCanton = datoInsertar.Canton
                };

                var resultInsertar = await _unitOfWork.DistritosRepository.CrearDistrito(distritoInsertar);
                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }
            await _unitOfWork.Save();

            return Result.Created;
        }

        private async Task<ErrorOr<Success>> EliminarDistritos(int idCanton)
        {
            var datosExistentes = await _unitOfWork.DistritosRepository.ObtenerDistritos();
            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            var distritosFiltrados = datosExistentes.Value
               .Where(b => b.IdCanton == idCanton)
               .ToList();


            foreach (var datoExistente in distritosFiltrados)
            {
                var resultadoDelete = await _unitOfWork.DistritosRepository.EliminarDistrito(datoExistente.Id);
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }
            await _unitOfWork.Save();
            return Result.Success;
        }

        private static bool ExistenDuplicados(IEnumerable<ImportarDistritoCommandDto> datos)
        {
            HashSet<string> datosComprobadosCodigo = new HashSet<string>();
            foreach (var distrito in datos)
            {
                if (!datosComprobadosCodigo.Add(distrito.Codigo))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
