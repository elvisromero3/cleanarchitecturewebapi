using ErrorOr;
using MediatR;
using System.Net.Http.Headers;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.ImportarDatos
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
                return ErroresSustanciaControlada.DatosDuplicadosArchivo;
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

        private static bool ExistenDuplicados(IEnumerable<ImportarSustanciasControladasCommandDto> datos)
        {
            HashSet<string> combinacionesUnicas = new HashSet<string>();

            foreach (var item in datos)
            {
                var clave = string.Join("|",
                    Validadores.NormalizarString(item.ClasificacionArancelaria),
                    Validadores.NormalizarString(item.ClasificacionAshrae),
                    Validadores.NormalizarString(item.PotencialCalentamientoGlobal),
                    Validadores.NormalizarString(item.Familia),
                    Validadores.NormalizarString(item.TipoGas)
                );

                // Si ya existe, hay duplicado
                if (!combinacionesUnicas.Add(clave))
                    return true;
            }

            return false;
        }
        private async Task<ErrorOr<Created>> EliminarDatosExistentes()
        {
            var datosExistentes = await _unitOfWork.SustanciaControladaRepository.ObtenerSustanciaControladas();

            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            foreach (var datoExistente in datosExistentes.Value)
            {
                var resultadoDelete = await _unitOfWork.SustanciaControladaRepository.EliminarSustanciaControlada(datoExistente.Id);

                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Created;
        }
        private async Task<ErrorOr<Created>> InsertarDatos(ImportarDatosCommand request)
        {
            var familias = await _unitOfWork.FamiliaRepository.ObtenerFamilias();


            foreach (var datoInsertar in request.Datos)
            {
                var familia = familias.Value.Find(x => x.Nombre == datoInsertar.Familia);
                if (familia is null)
                {
                    return ErroresSustanciaControlada.FamiliaNoEncontrada;
                }

                //se valida ClasificacionArancelaria
                if (Validadores.LongitudMaximaNoNull(datoInsertar.ClasificacionArancelaria, 100))
                {
                    return ErroresSustanciaControlada.SustanciaControladaClasificacionArancelariaTamano;
                }
                //se valida ClasificacionAshrae
                if (Validadores.LongitudMaximaNoNull(datoInsertar.ClasificacionAshrae, 100))
                {
                    return ErroresSustanciaControlada.SustanciaControladaClasificacionAshraeTamano;
                }
                //se valida PotencialCalentamientoGlobal
                if (Validadores.LongitudMaximaNoNull(datoInsertar.PotencialCalentamientoGlobal, 50))
                {
                    return ErroresSustanciaControlada.SustanciaControladaPotencialCalentamientoGlobalTamano;
                }
                //se valida TipoGas
                if (Validadores.LongitudMaximaNoNull(datoInsertar.TipoGas, 100))
                {
                    return ErroresSustanciaControlada.SustanciaControladaTipoGasTamano;
                }

                var sustanciaControlada = new Dominio.Entidades.SustanciaControlada
                {
                    ClasificacionArancelaria = datoInsertar.ClasificacionArancelaria,
                    ClasificacionAshrae = datoInsertar.ClasificacionAshrae,
                    PotencialCalentamientoGlobal = datoInsertar.PotencialCalentamientoGlobal,
                    IdFamilia = familia.Id,
                    TipoGas = datoInsertar.TipoGas
                };

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {

                    var existe = await _unitOfWork.SustanciaControladaRepository.ValidarSustanciasControladas(0, sustanciaControlada);
                    if (existe.Value)
                    {
                        return ErroresSustanciaControlada.DatosDuplicados;
                    }
                }

                var resultInsertar = await _unitOfWork.SustanciaControladaRepository.CrearSustanciaControlada(sustanciaControlada);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            return Result.Created;
        }
    }
}