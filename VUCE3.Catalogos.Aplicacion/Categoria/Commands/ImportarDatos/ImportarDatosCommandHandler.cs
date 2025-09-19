using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Categoria.Commands.ImportarDatos
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
                return ErroresCategoria.DatosDuplicadosArchivo;
            }

            var insertar = await InsertarDatos(request);
            if (insertar.IsError)
            {
                return insertar.Errors;
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private static bool ExistenDuplicados(IEnumerable<ImportarCategoriaCommandDto> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var categoria in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(categoria.Nombre) + "_" + categoria.Institucion))
                {
                    return true;
                }
            }

            return false;
        }
        private async Task<ErrorOr<Created>> EliminarDatosExistentes(int? idInstitucion)
        {
            var tipoProductos = await _unitOfWork.TipoProductoRepository.ObtenerTipoProductos();

            var datosExistentes = await _unitOfWork.CategoriaRepository.ObtenerCategorias();

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

            foreach (var categoriaId in datosExistentes.Value.Select(categoria => categoria.Id))
            {
                var tipoProducto = tipoProductos.Value.Count(x => x.IdCategoria == categoriaId);
                if (tipoProducto > 0)
                {
                    return ErroresCategoria.TipoProductoRelacionado;
                }

                var resultadoDelete = await _unitOfWork.CategoriaRepository.EliminarCategoria(categoriaId);

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
                return ErroresCategoria.FalloServicioAccesoGestionUsuario;
            }
            
            foreach (var datoInsertar in request.Datos)
            {
                var institucion = instituciones.Value.Find(x => x.Nombre == datoInsertar.Institucion || x.Id == datoInsertar.IdInstitucion);
               
                if (institucion is null || Validadores.institucionDCAoDIPOA(institucion.Id))
                {
                    return ErroresCategoria.CategoriaInstitucionInvalida;
                }

                if (Validadores.LongitudMaximaNoNull(datoInsertar.Nombre, 100))
                {
                    return ErroresCategoria.NombreInvalido;
                }

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.CategoriaRepository.ValidarCategoria(0, datoInsertar.Nombre, institucion.Id);
                    if (existe.Value)
                    {
                        return ErroresCategoria.DatosDuplicados;
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
                
                var categoria = new Dominio.Entidades.Categoria
                {
                    Nombre = datoInsertar.Nombre,
                    IdInstitucion = institucion.Id,
                };

                var resultInsertar = await _unitOfWork.CategoriaRepository.CrearCategoria(categoria);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            return Result.Created;
        }
    }
}