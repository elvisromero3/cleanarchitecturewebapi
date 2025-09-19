using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Variedad.Commands.CrearVariedad
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
                return ErroresVariedad.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var datosExistentes = await _unitOfWork.VariedadesRepository.ObtenerVariedades();

                if (datosExistentes.IsError)
                {
                    return datosExistentes.Errors;
                }

                foreach(var datoExistente in datosExistentes.Value)
                {
                    var resultadoDelete = await _unitOfWork.VariedadesRepository.EliminarVariedad(datoExistente);

                    if (resultadoDelete.IsError)
                    {
                        return resultadoDelete.Errors;
                    }
                }

                await _unitOfWork.Save();
            }

            foreach (var datoInsertar in request.Datos)
            {

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.VariedadesRepository.ValidarVariedad(datoInsertar.Id,datoInsertar.Codigo,datoInsertar.Nombre);

                    if (existe.Value)
                    {
                        return ErroresVariedad.DatosDuplicados;
                    }
                }

                if (datoInsertar.Codigo.Length > 17)
                {
                    return ErroresVariedad.VariedadCodigoInvalido;
                }

                if (datoInsertar.Nombre.Length > 100)
                {
                    return ErroresVariedad.VariedadNombreInvalido;
                }

                await _unitOfWork.VariedadesRepository.CrearVariedad(datoInsertar);

            }
            await _unitOfWork.Save();

            return Result.Created;
        }

        private static bool ExistenDuplicados(IEnumerable<Dominio.Entidades.Variedad> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var variedad in datos)
            {
                if (!datosComprobados.Add(variedad.Codigo + "_" + Validadores.NormalizarString(variedad.Nombre)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
