using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Provincias.Commands.ImportarDatos
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
                return ErroresProvincia.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var resultadoDelete = await EliminarProvincias();
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            foreach (var datoInsertar in request.Datos)
            {
                if (Validadores.Codigo1(datoInsertar.Codigo))
                {
                    return ErroresProvincia.ProvinciaCodigoInvalido;
                }

                if (Validadores.Nombre50(datoInsertar.Nombre))
                {
                    return ErroresProvincia.ProvinciaNombreInvalido;
                }

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.ProvinciaRepository.ValidarProvincia(datoInsertar.Id, datoInsertar.Codigo);
                    if (existe.Value)
                    {
                        return ErroresProvincia.DatosDuplicados;
                    }
                }
                
                var resultInsertar = await _unitOfWork.ProvinciaRepository.CrearProvincia(datoInsertar);
                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private async Task<ErrorOr<Success>> EliminarProvincias()
        {
            var datosExistentes = await _unitOfWork.ProvinciaRepository.ObtenerProvincias();
            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            foreach (var datoExistenteId in datosExistentes.Value.Select(datoExistente => datoExistente.Id))
            {
                // Verifica si provincia tiene relacion con Canton
                var existeRelacionCanton = await _unitOfWork.ProvinciaRepository.ExisteRelacionConCanton(datoExistenteId);
               
                if (existeRelacionCanton.IsError)
                {
                    return existeRelacionCanton.Errors;
                }

                if (existeRelacionCanton.Value)
                {
                    return ErroresProvincia.ProvinciaRelacionCanton;
                }

                var resultadoDelete = await _unitOfWork.ProvinciaRepository.EliminarProvincia(datoExistenteId);
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Success;
        }

        private static bool ExistenDuplicados(IEnumerable<Dominio.Entidades.Provincia> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var provincia in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(provincia.Codigo)))
                {
                    return true;
                }
            }
            return false;
        }
     
    }
}
