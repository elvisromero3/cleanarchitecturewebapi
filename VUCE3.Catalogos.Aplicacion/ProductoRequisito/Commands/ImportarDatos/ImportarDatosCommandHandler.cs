using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.ImportarDatos
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
                return ErroresProductoRequisito.RequisitoDatosDuplicadosArchivo;
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

        private static bool ExistenDuplicados(IEnumerable<ImportarProductoRequisitoCommandDto> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var requisito in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(requisito.TipoProducto)))
                {
                    return true;
                }
            }

            return false;
        }
        private async Task<ErrorOr<Created>> EliminarDatosExistentes()
        {
            var datosExistentes = await _unitOfWork.ProductoRequisitoRepository.ObtenerProductoRequisitos();

            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            foreach (var datoExistente in datosExistentes.Value)
            {
                var resultadoDelete = await _unitOfWork.ProductoRequisitoRepository.EliminarProductoRequisito(datoExistente.Id);

                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Created;
        }
        private async Task<ErrorOr<Created>> InsertarDatos(ImportarDatosCommand request)
        {
            var tipoProductos = await _unitOfWork.TipoProductoRepository.ObtenerTipoProductos();

            var requisitos = await _unitOfWork.RequisitosRepository.ObtenerRequisitos();

            foreach (var datoInsertar in request.Datos)
            {
                var tipoproducto = tipoProductos.Value.Find(x => x.Tipo == datoInsertar.TipoProducto);
                if (tipoproducto is null)
                {
                    return ErroresProductoRequisito.TipoProductoNoEncontrada;
                }

                var existeRequisito = requisitos.Value.Find(x => x.Id ==datoInsertar.IdRequisito);
                if (existeRequisito is null)
                {
                    return ErroresProductoRequisito.RequisitoNoEncontrado;                
                }

                if (tipoproducto.IdInstitucion != existeRequisito.IdInstitucion)
                {
                    return ErroresProductoRequisito.TipoProductoInstitucion;
                }

                var productorequisito = new Dominio.Entidades.ProductoRequisito { 
                   IdRequisito = datoInsertar.IdRequisito,
                   IdTipoProducto = tipoproducto.Id
                };

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var productoRequisitoExistente = await _unitOfWork.ProductoRequisitoRepository.ValidarProductoRequisitos(0, productorequisito.IdTipoProducto, productorequisito.IdRequisito);
                    if (productoRequisitoExistente.Value)
                    {
                        return ErroresProductoRequisito.ProductoRequisitoDuplicados;
                    }
                }

                var resultInsertar = await _unitOfWork.ProductoRequisitoRepository.CrearProductoRequisito(productorequisito);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            return Result.Created;
        }
    }
}