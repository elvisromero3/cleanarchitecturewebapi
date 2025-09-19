using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Commands.ImportarDatos
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
                return ErroresCultivo.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var resultadoDelete = await eliminarCultivos();
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
                
            }

            foreach (var datoInsertar in request.Datos)
            {
                
                if (datoInsertar.Codigo.Length > 25)
                {
                    return ErroresCultivo.CultivoCodigoInvalido;
                }

                if (datoInsertar.Nombre.Length > 100)
                {
                    return ErroresCultivo.CultivoNombreInvalido;
                }

                if (datoInsertar.NombreCientifico.Length > 100)
                {
                    return ErroresCultivo.CultivoNombreCientificoInvalido;
                }

                var variedad = await _unitOfWork.VariedadesRepository.ObtenerVariedadPorCodigo(datoInsertar.CodigoVariedad);
                if (variedad.IsError)
                {
                    return variedad.Errors;
                }

                Dominio.Entidades.Cultivo cultivo = new Dominio.Entidades.Cultivo
                {
                    Codigo = datoInsertar.Codigo,
                    Nombre = datoInsertar.Nombre,
                    NombreCientifico = datoInsertar.NombreCientifico,
                    IdVariedad = variedad.Value.Id
                };

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.CultivosRepository.ValidarCultivo(0, cultivo.Codigo, cultivo.Nombre, cultivo.NombreCientifico);

                    if (existe.Value)
                    {
                        return ErroresCultivo.DatosDuplicados;
                    }
                }

                var resultInsertar = await _unitOfWork.CultivosRepository.CrearCultivo(cultivo);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private async Task<ErrorOr<Success>> eliminarCultivos()
        {
            var datosExistentes = await _unitOfWork.CultivosRepository.ObtenerCultivos();

            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            foreach (var datoExistente in datosExistentes.Value)
            {
                var resultadoDelete = await _unitOfWork.CultivosRepository.EliminarCultivo(datoExistente.Id);

                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Success;
        }

        private static bool ExistenDuplicados(IEnumerable<ImportarCultivoCommandDto> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var cultivo in datos)
            {
                if (!datosComprobados.Add(cultivo.Codigo + "_" + 
                    Validadores.NormalizarString(cultivo.Nombre) + "_" + 
                    Validadores.NormalizarString(cultivo.NombreCientifico)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
