using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Commands.ImportarDatos
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
                return ErroresSector.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var resultadoDelete = await eliminarSectores();
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            foreach (var datoInsertar in request.Datos)
            {
                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.SectoresRepository.ValidarSector(0, datoInsertar.Nombre, datoInsertar.Codigo);

                    if (existe.Value)
                    {
                        return ErroresSector.DatosDuplicados;
                    }
                }
                if (Validadores.LongitudMaximaNoNull(datoInsertar.Nombre, 50))
                {
                    return ErroresSector.SectorNombreInvalido;
                }
                if (Validadores.LongitudMaximaNoNull(datoInsertar.Codigo, 10))
                {
                    return ErroresSector.SectorCodigoInvalido;
                }
                var resultInsertar = await _unitOfWork.SectoresRepository.CrearSector(new Dominio.Entidades.Sector {Nombre = datoInsertar.Nombre, Codigo = datoInsertar.Codigo});
                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private async Task<ErrorOr<Success>> eliminarSectores()
        {
            var datosExistentes = await _unitOfWork.SectoresRepository.ObtenerSectores();
            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            foreach (var sector in datosExistentes.Value)
            {
                var resultadoDelete = await _unitOfWork.SectoresRepository.EliminarSector(sector.Id);
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Success;
        }

        private static bool ExistenDuplicados(IEnumerable<ImportarSectorCommandDto> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();

            foreach (var sector in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(sector.Codigo)+Validadores.NormalizarString(sector.Nombre)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
