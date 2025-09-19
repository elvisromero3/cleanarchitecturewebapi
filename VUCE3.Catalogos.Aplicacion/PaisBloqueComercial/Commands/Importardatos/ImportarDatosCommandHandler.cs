using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.Importardatos
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
                return ErroresPaisBloqueComercial.PaisBloqueComercialDatosDuplicadosArchivo;
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

        private async Task<ErrorOr<Created>> EliminarDatosExistentes()
        {
            var datosExistentes = await _unitOfWork.PaisBloqueComercialRepository.ObtenerPaisBloqueComercial();

            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            foreach (var datoExistente in datosExistentes.Value)
            {
                var resultadoDelete = await _unitOfWork.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(datoExistente.Id);

                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Created;
        }

        private static bool ExistenDuplicados(IEnumerable<ImportarPaisBloqueComercialCommandDto> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var paisBloqueComercial in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(paisBloqueComercial.BloqueComercial) + "_"  + Validadores.NormalizarString(paisBloqueComercial.Pais)
                    ))
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<ErrorOr<Created>> InsertarDatos(ImportarDatosCommand request)
        {
            var paises = await _unitOfWork.PaisesRepository.ObtenerPaises();
            var bloqueComerciales = await _unitOfWork.BloqueComercialRepository.ObtenerBloquesComerciales();

            foreach (var datoInsertar in request.Datos)
            {
                var pais = paises.Value.Find(x => x.Nombre == datoInsertar.Pais);
                if (pais is null)
                {
                    return ErroresPaisBloqueComercial.PaisNoEncontrado;
                }

                var bloqueComercial = bloqueComerciales.Value.Find(x => x.Nombre == datoInsertar.BloqueComercial);
                if (bloqueComercial is null)
                {
                    return ErroresPaisBloqueComercial.BloqueComercialNoEncontrado;
                }

                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.PaisBloqueComercialRepository.ValidarPaisBloqueComercial(bloqueComercial.Id, pais.Id);
                    if (existe.Value)
                    {
                        return ErroresPaisBloqueComercial.DatosDuplicados;
                    }
                }

                var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial
                {
                    IdBloqueComercial = bloqueComercial.Id,
                    IdPais = pais.Id
                };

                var resultInsertar = await _unitOfWork.PaisBloqueComercialRepository.CrearPaisBloqueComercial(paisBloqueComercial);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            return Result.Created;
        }
    }
}
