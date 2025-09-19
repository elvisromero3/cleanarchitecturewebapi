using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Commands.ImportarDatos
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
                return ErroresAduana.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var datosExistentes = await _unitOfWork.AduanasRepository.ObtenerAduanas();

                if (datosExistentes.IsError)
                {
                    return datosExistentes.Errors;
                }

                foreach (var datoExistente in datosExistentes.Value)
                {
                    var resultadoDelete = await _unitOfWork.AduanasRepository.EliminarAduana(datoExistente.Id);

                    if (resultadoDelete.IsError)
                    {
                        return resultadoDelete.Errors;
                    }
                }
            }

            foreach (var datoInsertar in request.Datos)
            {                

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.AduanasRepository.ValidarAduana(datoInsertar);

                    if (existe.Value)
                    {
                        return ErroresAduana.DatosDuplicados;
                    }
                }

                if (datoInsertar.Nombre.Length > 100)
                {
                    return ErroresAduana.AduanaNombreInvalido;
                }

                var resultInsertar = await _unitOfWork.AduanasRepository.CrearAduana(datoInsertar);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private static bool ExistenDuplicados(IEnumerable<Aduana> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var aduana in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(aduana.Nombre)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}