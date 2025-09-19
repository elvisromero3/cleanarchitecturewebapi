using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.ImportarDatos.DTO;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Commands.ImportarDatos
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
                return ErroresEmpresas.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var datosExistentes = await _unitOfWork.EmpresasRepository.ObtenerEmpresasPorIdProfesional(request.Datos.First().IdProfesional);

                if (datosExistentes.IsError)
                {
                    return datosExistentes.Errors;
                }

                foreach (var datoExistente in datosExistentes.Value)
                {
                    var resultadoDelete = await _unitOfWork.EmpresasRepository.EliminarEmpresa(datoExistente.Id);

                    if (resultadoDelete.IsError)
                    {
                        return resultadoDelete.Errors;
                    }
                }
            }

            foreach (var datoInsertar in request.Datos)
            {
                var empresa = new Dominio.Entidades.Empresa()
                {
                    IdProfesional = datoInsertar.IdProfesional,
                    Nombre = datoInsertar.Nombre,
                    IdTipoIdentificacion = datoInsertar.TipoIdentificacion,
                    NumeroIdentificacion = datoInsertar.NumeroIdentificacion
                };

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.EmpresasRepository.ValidarEmpresa(empresa.Id, empresa.IdTipoIdentificacion, empresa.NumeroIdentificacion, empresa.IdProfesional);

                    if (existe.Value)
                    {
                        return ErroresEmpresas.DatosDuplicados;
                    }
                }

                if (datoInsertar.Nombre.Length > 100)
                {
                    return ErroresEmpresas.EmpresaNombreExcedeLimite;
                }

                if (datoInsertar.TipoIdentificacion != ConstantesIdTipoIdentificacion.FISICA &&
                    datoInsertar.TipoIdentificacion != ConstantesIdTipoIdentificacion.JURIDICA &&
                    datoInsertar.TipoIdentificacion != ConstantesIdTipoIdentificacion.DIMEX &&
                    datoInsertar.TipoIdentificacion != ConstantesIdTipoIdentificacion.PASAPORTE)
                {
                    return ErroresEmpresas.TipoIdentificacionNoEncontrado;
                }

                if (Validadores.NumeroIdentificacion(datoInsertar.TipoIdentificacion, datoInsertar.NumeroIdentificacion))
                {
                    return ErroresEmpresas.NumeroIdentificacionExcedeLimite;
                }

                //se valida identificación física comienza con 0
                if (Validadores.TipoIdentificacionFisicaComienza0(datoInsertar.TipoIdentificacion, datoInsertar.NumeroIdentificacion))
                {
                    return ErroresEmpresas.TipoIdentificacionFisicaComienza0;
                }

                var resultInsertar = await _unitOfWork.EmpresasRepository.CrearEmpresa(empresa);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private static bool ExistenDuplicados(IEnumerable<ImportarEmpresaCommandDto> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var empresa in datos)
            {
                if (!datosComprobados.Add(empresa.NumeroIdentificacion + "_" + empresa.TipoIdentificacion + "_" + empresa.IdProfesional))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
