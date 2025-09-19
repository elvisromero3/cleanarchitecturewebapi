using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.BloquesComerciales.Commands.Importardatos
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
                return ErroresBloqueComercial.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var resultadoDelete = await eliminarBloquesComerciales();
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            foreach (var datoInsertar in request.Datos)
            {
                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.BloqueComercialRepository.ValidarBloqueComercial(datoInsertar.Id, datoInsertar.Nombre);

                    if (existe.Value)
                    {
                        return ErroresBloqueComercial.DatosDuplicados;
                    }
                }

                if (Validadores.LongitudMaximaNoNull(datoInsertar.Nombre, 300))
                {
                    return ErroresBloqueComercial.NombreInvalido;
                }

                var resultInsertar = await _unitOfWork.BloqueComercialRepository.CrearBloqueComercial(datoInsertar);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private async Task<ErrorOr<Success>> eliminarBloquesComerciales()
        {
            var datosExistentes = await _unitOfWork.BloqueComercialRepository.ObtenerBloquesComerciales();

            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            foreach (var datoExistente in datosExistentes.Value)
            {
                var resultadoDelete = await _unitOfWork.BloqueComercialRepository.EliminarBloqueComercial(datoExistente.Id);

                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Success;
        }

        private static bool ExistenDuplicados(IEnumerable<Dominio.Entidades.BloqueComercial> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var bloque in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(bloque.Nombre)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
