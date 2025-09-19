using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Familia.Commands.ImportarDatos
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
                return ErroresFamilia.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var eliminar = await EliminarDatosExistentes();
                if (eliminar.IsError)
                {
                    return eliminar.Errors;
                }
            }

            var insertar = await InsertarDatos(request);
            if (insertar.IsError)
            {
                return insertar.Errors;
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private static bool ExistenDuplicados(IEnumerable<Dominio.Entidades.Familia> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var familia in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(familia.Nombre)))
                {
                    return true;
                }
            }

            return false;
        }
        private async Task<ErrorOr<Created>> EliminarDatosExistentes()
        {
            var sustancias = await _unitOfWork.SustanciaControladaRepository.ObtenerSustanciaControladas();

            var datosExistentes = await _unitOfWork.FamiliaRepository.ObtenerFamilias();
            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            foreach (var datoExistente in datosExistentes.Value)
            {
                var sustanciasPorFamilia = sustancias.Value.Count(x => x.IdFamilia == datoExistente.Id);
                if (sustanciasPorFamilia > 0)
                {
                    return ErroresFamilia.SustanciasControladasRelacionadas;
                }

                var resultadoDelete = await _unitOfWork.FamiliaRepository.EliminarFamilia(datoExistente.Id);

                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Created;
        }
        private async Task<ErrorOr<Created>> InsertarDatos(ImportarDatosCommand request)
        {
            foreach (var datoInsertar in request.Datos)
            {
                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.FamiliaRepository.ValidarFamilia(datoInsertar.Id, datoInsertar.Nombre);

                    if (existe.Value)
                    {
                        return ErroresFamilia.DatosDuplicados;
                    }
                }

                if (string.IsNullOrWhiteSpace(datoInsertar.Nombre) || datoInsertar.Nombre.Length > 50)
                {
                    return ErroresFamilia.NombreInvalido;
                }

                var resultInsertar = await _unitOfWork.FamiliaRepository.CrearFamilia(datoInsertar);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            return Result.Created;
        }
    }
}