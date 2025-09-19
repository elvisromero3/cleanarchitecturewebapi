using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Servicios;

namespace VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.ImportarDatos
{
    public class ImportarDatosCommandHandler : IRequestHandler<ImportarDatosCommand, ErrorOr<Created>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccesoGestionUsuariosService _accesoUsuariosService;
        public ImportarDatosCommandHandler(IUnitOfWork unitOfWork, IAccesoGestionUsuariosService accesoUsuariosService)
        {
            _unitOfWork = unitOfWork;
            _accesoUsuariosService = accesoUsuariosService;
        }
        public async Task<ErrorOr<Created>> Handle(ImportarDatosCommand request, CancellationToken cancellationToken)
        {
            if (ExistenDuplicados(request.Datos))
            {
                return ErroresTipoProducto.DatosDuplicadosArchivo;
            }

            var insertar = await InsertarDatos(request);
            if (insertar.IsError)
            {
                return insertar.Errors;
            }

            await _unitOfWork.Save();
            return Result.Created;
        }

        private static bool ExistenDuplicados(IEnumerable<ImportarTipoProductoCommandDto> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var tipoProducto in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(tipoProducto.Categoria) + "_" + Validadores.NormalizarString(tipoProducto.Tipo)))
                {
                    return true;
                }
            }
            return false;
        }
        private async Task<ErrorOr<Created>> EliminarDatosExistentes(int? idInstitucion)
        {
            var datosExistentes = await _unitOfWork.TipoProductoRepository.ObtenerTipoProductos();
            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            // si la institucion es null, significa que se está realizando desde PROCOMER, por lo que no debemos filtrar pos institución
            if (idInstitucion is not null)
            {
                datosExistentes = datosExistentes.Value
                .Where(b => b.IdInstitucion == idInstitucion)
                .ToList();
            }

            foreach (var tipoProductoId in datosExistentes.Value.Select(tipoProducto => tipoProducto.Id))
            {
                var productoRequisitoExite = await _unitOfWork.TipoProductoRepository.ValidarExisteProductoRequisito(tipoProductoId);
                if (productoRequisitoExite.Value)
                {
                    return ErroresTipoProducto.ProductoRequisitoExiste;
                }

                var resultadoDelete = await _unitOfWork.TipoProductoRepository.EliminarTipoProducto(tipoProductoId);
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Created;
        }
        private async Task<ErrorOr<Created>> InsertarDatos(ImportarDatosCommand request)
        {
            var instituciones = await _accesoUsuariosService.ObtenerInstituciones();
            if (instituciones.IsError)
            {
                return ErroresTipoProducto.FalloServicioAccesoGestionUsuario;
            }

            var categorias = await _unitOfWork.CategoriaRepository.ObtenerCategorias();
            var listaCategorias = categorias.Value;

            foreach (var datoInsertar in request.Datos)
            {
                var institucion = instituciones.Value.Find(x => x.Nombre == datoInsertar.Institucion || x.Id == datoInsertar.IdInstitucion);
                if (institucion is null)
                {
                    return ErroresTipoProducto.InstitucionNoEncontrado;
                }

                if (institucion.Id != ConstantesInstituciones.DCA && institucion.Id != ConstantesInstituciones.DIPOA)
                {
                    return ErroresTipoProducto.InstitucionNoValida;
                }

                if (Validadores.LongitudMaximaNoNull(datoInsertar.Tipo, 200))
                {
                    return ErroresTipoProducto.TipoTamano;
                }

                // Buscamos la categoría por nombre
                var categoriaPorNombre = listaCategorias.Where(c =>
                    Validadores.NormalizarString(c.Nombre) == Validadores.NormalizarString(datoInsertar.Categoria));

                if (!categoriaPorNombre.Any())
                {
                    return ErroresTipoProducto.CategoriaNoEncontrado;
                }

                var categoriaPorInstitucion = categoriaPorNombre.FirstOrDefault(c => c.IdInstitucion == institucion.Id);

                // Verificamos si pertenece a la institución seleccionada
                if (categoriaPorInstitucion is null)
                {
                    return ErroresTipoProducto.CategoriaPerteneceAOtraInstitucion;
                }

                var tipoProducto = new Dominio.Entidades.TipoProducto
                {
                    IdCategoria = categoriaPorInstitucion.Id,
                    Tipo = datoInsertar.Tipo,
                    IdInstitucion = institucion.Id,
                };

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.TipoProductoRepository.ValidarTipoProducto(tipoProducto.Tipo, tipoProducto.IdCategoria, tipoProducto.IdInstitucion);
                    if (existe.Value)
                    {
                        return ErroresTipoProducto.DatosDuplicados;
                    }
                }
                else
                {
                    var resultadoDelete = await EliminarDatosExistentes(datoInsertar.IdInstitucion);
                    if (resultadoDelete.IsError)
                    {
                        return resultadoDelete.Errors;
                    }
                }

                var resultInsertar = await _unitOfWork.TipoProductoRepository.CrearTipoProducto(tipoProducto);
                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            return Result.Created;
        }
    }
}