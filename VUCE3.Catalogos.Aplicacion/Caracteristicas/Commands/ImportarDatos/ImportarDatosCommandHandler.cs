using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Aplicacion.Servicios.DTO;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.ImportarDatos
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
                return ErroresCaracteristica.DatosDuplicadosArchivo;
            }

            var instituciones = await _accesoUsuariosService.ObtenerInstituciones();

            foreach (var datoInsertar in request.Datos)
            {
                var institucion = instituciones.Value.Find(x => x.Nombre == datoInsertar.Institucion || x.Id == datoInsertar.IdInstitucion);
                if (institucion is null || Validadores.institucionDCAoDIPOA(institucion.Id))
                {
                    return ErroresCaracteristica.CaracteristicaInstitucionInvalida;
                }

                if (Validadores.LongitudMaximaNoNull(datoInsertar.Nombre, 150))
                {
                    return ErroresCaracteristica.CaracteristicaNombreInvalido;
                }

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.CaracteristicasRepository.ValidarCaracteristica(0, institucion.Id, datoInsertar.Nombre);

                    if (existe.Value)
                    {
                        return ErroresCaracteristica.CaracteristicaDatosDuplicados;
                    }
                }
                else
                {
                    var resultadoDelete = await EliminarCaracteristica(datoInsertar.IdInstitucion);
                    if (resultadoDelete.IsError)
                    {
                        return resultadoDelete.Errors;
                    }
                }

                var caracteristica = new Caracteristica
                {
                    Nombre = datoInsertar.Nombre,
                    IdInstitucion = institucion.Id
                };

                var resultInsertar = await _unitOfWork.CaracteristicasRepository.CrearCaracteristica(caracteristica);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private static bool ExistenDuplicados(IEnumerable<ImportarDatosCaracteristicaDto> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var caracteristica in datos)
            {
                if (!datosComprobados.Add(caracteristica.Institucion + "_" + Validadores.NormalizarString(caracteristica.Nombre)))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<ErrorOr<Success>> EliminarCaracteristica(int? idInstitucion)
        {
            var datosExistentes = await _unitOfWork.CaracteristicasRepository.ObtenerCaracteristicas();

            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            // si la institucion es null, significa que se está realizando desde PROCOMER, por lo que no debemos filtrar pos institución
            if(idInstitucion is not null)
            {
                datosExistentes = datosExistentes.Value
                .Where(b => b.IdInstitucion == idInstitucion)
                .ToList();
            }


            // Todo: Revisar si se puede hacer un bulk delete o hacer asincrono el foreach
            foreach (var datoExistenteId in datosExistentes.Value.Select(x=>x.Id))
            {
                var resultadoDelete = await _unitOfWork.CaracteristicasRepository.EliminarCaracteristica(datoExistenteId);

                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }
            await _unitOfWork.Save();
            return Result.Success;
        }
    }
}
