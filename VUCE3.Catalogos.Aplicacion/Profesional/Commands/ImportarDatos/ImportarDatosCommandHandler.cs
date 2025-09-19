using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Profesional.Commands.ImportarDatos
{
    public class ImportarDatosCommandHandler : IRequestHandler<ImportarDatosCommand, ErrorOr<Created>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccesoGestionUsuariosService _accesoUsuariosService;
        public ImportarDatosCommandHandler(IUnitOfWork unitOfWork,
            IAccesoGestionUsuariosService accesoUsuariosService)
        {
            _unitOfWork = unitOfWork;
            _accesoUsuariosService = accesoUsuariosService;
        }

        public async Task<ErrorOr<Created>> Handle(ImportarDatosCommand request, CancellationToken cancellationToken)
        {
            if (ExistenDuplicados(request.Datos))
            {
                return ErroresProfesionales.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var idInstitucion = request.Datos.Select(x => x.IdInstitucion).First();

                ErrorOr<List<Dominio.Entidades.Profesional>> datosExistentes;

                if (idInstitucion is null)
                {
                    datosExistentes = await _unitOfWork.ProfesionalesRepository.ObtenerProfesionales();
                }
                else
                {
                    datosExistentes = await _unitOfWork.ProfesionalesRepository.ObtenerProfesionalesPorIdInstitucion(idInstitucion.Value);
                }
               

                if (datosExistentes.IsError)
                {
                    return datosExistentes.Errors;
                }

                if (datosExistentes.Value is not null)
                {
                    foreach (var datoExistente in datosExistentes.Value)
                    {
                        var resultadoDelete = await _unitOfWork.ProfesionalesRepository.EliminarProfesional(datoExistente.Id);

                        if (resultadoDelete.IsError)
                        {
                            return resultadoDelete.Errors;
                        }
                    }
                }
            }


            var instituciones = await _accesoUsuariosService.ObtenerInstituciones();
            if(instituciones.IsError)
            {
                return ErroresProfesionales.FalloServicioAccesoGestionUsuario;  
            }

            foreach (var datoInsertar in request.Datos)
            {
                var institucion = instituciones.Value.Find(x => x.Nombre == datoInsertar.Institucion || x.Id == datoInsertar.IdInstitucion);
                if (institucion is null)
                {
                    return ErroresProfesionales.InstitucionNoEncontrado;
                }

                var profesional = new Dominio.Entidades.Profesional()
                {
                    Nombre = datoInsertar.Nombre,
                    CodigoRegente = datoInsertar.CodigoRegente,
                    IdTipoIdentificacion = datoInsertar.TipoIdentificacion,
                    NumeroIdentificacion = datoInsertar.NumeroIdentificacion,
                    Profesion = datoInsertar.Profesion,
                    Activo = datoInsertar.Activo!.Value,
                    Email = datoInsertar.Email,
                    IdInstitucion = datoInsertar.IdInstitucion.HasValue ? datoInsertar.IdInstitucion!.Value : institucion.Id
                };

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.ProfesionalesRepository.ValidarProfesional(profesional.Id, profesional.NumeroIdentificacion, profesional.IdInstitucion);
                    if (existe.Value)
                    {
                        return ErroresProfesionales.DatosDuplicados;
                    }
                }

                //se valida tamaño email
                if (datoInsertar.Email.Length > 100)
                {
                    return ErroresProfesionales.TamanoEmail;
                }

                //se valida formato de email
                if (Validadores.EmailInvalido(datoInsertar.Email))
                {
                    return ErroresProfesionales.EmailInvalido;
                }
                
                //se valida tipo identificación
                if (datoInsertar.TipoIdentificacion != ConstantesIdTipoIdentificacion.FISICA &&
                    datoInsertar.TipoIdentificacion != ConstantesIdTipoIdentificacion.DIMEX)
                {
                    return ErroresProfesionales.TipoIdentificacionIncorrecta;
                }

                //se valida tipo identificación y tamaño
                if (Validadores.NumeroIdentificacion(datoInsertar.TipoIdentificacion, datoInsertar.NumeroIdentificacion))
                {
                    return ErroresProfesionales.TamanoNroIdentificacion;
                }

                //se valida identificación física comienza con 0
                if (Validadores.TipoIdentificacionFisicaComienza0(datoInsertar.TipoIdentificacion, datoInsertar.NumeroIdentificacion))
                {
                    return ErroresProfesionales.TipoIdentificacionFisicaComienza0;
                }

                //se valida tamaño de nombre
                if (datoInsertar.Nombre.Length > 100)
                {
                    return ErroresProfesionales.TamanoNombre;
                }

                //se valida tamaño profesión
                if (datoInsertar.Profesion.Length > 100)
                {
                    return ErroresProfesionales.TamanoProfesion;
                }

                //se valida tamaño código regente
                if (datoInsertar.CodigoRegente.Length > 20)
                {
                    return ErroresProfesionales.TamanoCodigoRegente;
                }

                var resultInsertar = await _unitOfWork.ProfesionalesRepository.CrearProfesional(profesional);
                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();
            return Result.Created;
        }
        private static bool ExistenDuplicados(IEnumerable<ImportarProfesionalCommandDto> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var profesional in datos)
            {
                if (!datosComprobados.Add(profesional.NumeroIdentificacion+"_"+profesional.TipoIdentificacion + "_"+profesional.Institucion))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
