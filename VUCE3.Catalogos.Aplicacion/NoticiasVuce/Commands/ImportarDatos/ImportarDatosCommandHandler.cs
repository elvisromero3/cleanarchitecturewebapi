using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.ImportarDatos
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
                return ErroresNoticiasVuce.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var datosExistentes = await _unitOfWork.NoticiasVuceRepository.ObtenerNoticiasVuce();

                if (datosExistentes.IsError)
                {
                    return datosExistentes.Errors;
                }

                foreach (var datoExistente in datosExistentes.Value)
                {
                    var resultadoDelete = await _unitOfWork.NoticiasVuceRepository.EliminarNoticiasVuce(datoExistente.Id);

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
                    var existe = await _unitOfWork.NoticiasVuceRepository.ValidarNoticiasVuce(datoInsertar.Titulo,datoInsertar.Texto,datoInsertar.Enlace,datoInsertar.TituloIngles,datoInsertar.TextoIngles);
                    if (existe.Value)
                    {
                        return ErroresNoticiasVuce.DatosDuplicados;
                    }
                }

                if (datoInsertar.Titulo.Length > 100)
                {
                    return ErroresNoticiasVuce.NoticiasVuceTituloInvalido;
                }

                if (datoInsertar.TituloIngles.Length > 100)
                {
                    return ErroresNoticiasVuce.NoticiasVuceTituloInglesInvalido;
                }

                if (datoInsertar.Enlace is not null)
                {
                    if(Validadores.Enlace(datoInsertar.Enlace))
                    {
                        return ErroresNoticiasVuce.NoticiasLinkExcedeLimite;
                    }
                    if (Validadores.UrlInvalida(datoInsertar.Enlace))
                    {
                        return ErroresNoticiasVuce.NoticiasLinkInValido;
                    }
                }

                var resultInsertar = await _unitOfWork.NoticiasVuceRepository.CrearNoticiasVuce(datoInsertar);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private static bool ExistenDuplicados(IEnumerable<Dominio.Entidades.NoticiasVuce> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var noticiaVuce in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(noticiaVuce.Titulo + "_" + 
                    noticiaVuce.TituloIngles + "_" + 
                    noticiaVuce.Texto + "_" + 
                    noticiaVuce.TextoIngles)+"_"+
                    (noticiaVuce.Enlace??"")))
                {
                    return true;
                }
            }

            return false;
        }
    }
}