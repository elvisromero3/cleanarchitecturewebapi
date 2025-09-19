using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Casa.Commands.ImportarDatos
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
                return ErroresCasa.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var resultadoDelete = await eliminarCasas();
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }

            }

            foreach (var datoInsertar in request.Datos)
            {
                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.CasaRepository.ValidarCasa(datoInsertar.Id, datoInsertar.Codigo, datoInsertar.Nombre);

                    if (existe.Value)
                    {
                        return ErroresCasa.DatosDuplicados;
                    }
                }

                if (datoInsertar.Codigo.Length > 17)
                {
                    return ErroresCasa.CasaCodigoInvalido;
                }


                if (datoInsertar.Nombre.Length > 100)
                {
                    return ErroresCasa.CasaNombreInvalido;
                }

                var resultInsertar = await _unitOfWork.CasaRepository.CrearCasa(datoInsertar);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private async Task<ErrorOr<Success>> eliminarCasas()
        {
            var datosExistentes = await _unitOfWork.CasaRepository.ObtenerCasas();

            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            foreach (var datoExistente in datosExistentes.Value)
            {
                var resultadoDelete = await _unitOfWork.CasaRepository.EliminarCasa(datoExistente.Id);

                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Success;
        }

        private static bool ExistenDuplicados(IEnumerable<Dominio.Entidades.Casa> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var casa in datos)
            {
                if (!datosComprobados.Add(casa.Codigo+"_"+ Validadores.NormalizarString(casa.Nombre)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
