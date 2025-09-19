using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.ImportarDatos
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
                return ErroresCaracteristicaTipoProducto.DatosDuplicadosArchivo;
            }

            var insertar = await InsertarDatos(request);
            if (insertar.IsError)
            {
                return insertar.Errors;
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private static bool ExistenDuplicados(IEnumerable<ImportarCaracteristicaTipoProductoCommandDto> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var caracteristicaTipoProducto in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(caracteristicaTipoProducto.Caracteristica)))
                {
                    return true;
                }
            }

            return false;
        }
        private async Task<ErrorOr<Created>> EliminarDatosExistentes(int idTipoProducto)
        {
            var datosExistentes = await _unitOfWork.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductos();

            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            var CaracteristicaTipoProductosFiltrados = datosExistentes.Value
            .Where(b => b.IdTipoProducto == idTipoProducto)
            .ToList();

            foreach (var datoExistente in CaracteristicaTipoProductosFiltrados)
            {
                var resultadoDelete = await _unitOfWork.CaracteristicaTipoProductoRepository.EliminarCaracteristicaTipoProducto(datoExistente.Id);

                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Created;
        }
        private async Task<ErrorOr<Created>> InsertarDatos(ImportarDatosCommand request)
        {
            var caracteristicas = await _unitOfWork.CaracteristicasRepository.ObtenerCaracteristicas();
            var tiposProductos = await _unitOfWork.TipoProductoRepository.ObtenerTipoProductos();

            foreach (var datoInsertar in request.Datos)
            {
                var tipoProducto = tiposProductos.Value.Find(x => x.Id == datoInsertar.IdTipoProducto);
                var caracteristicasTipoProducto = caracteristicas.Value.Where(x => x.Nombre == datoInsertar.Caracteristica).ToList();

                if (caracteristicasTipoProducto.Count == 0)
                {
                    return ErroresCaracteristicaTipoProducto.CaracteristicaNoEncontrada;
                }

                var caracteristica = caracteristicasTipoProducto.Find(x => x.IdInstitucion == tipoProducto?.IdInstitucion);

                if (caracteristica is null)
                {
                    return ErroresCaracteristicaTipoProducto.CaracteristicaNoPerteneceInstitucion;
                }

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                   
                    var existe = await _unitOfWork.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(datoInsertar.IdTipoProducto, caracteristica.Id);
                    if (existe.Value)
                    {
                        return ErroresCaracteristicaTipoProducto.DatosDuplicados;
                    }
                }
                else
                {
                    var resultadoDelete = await EliminarDatosExistentes(datoInsertar.IdTipoProducto);
                    if (resultadoDelete.IsError)
                    {
                        return resultadoDelete.Errors;
                    }
                }

                var caracteristicaTipoproducto = new Dominio.Entidades.CaracteristicaTipoProducto { IdTipoProducto = datoInsertar.IdTipoProducto, IdCaracteristica = caracteristica.Id };

                var resultInsertar = await _unitOfWork.CaracteristicaTipoProductoRepository.CrearCaracteristicaTipoProducto(caracteristicaTipoproducto);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            return Result.Created;
        }
    }
}