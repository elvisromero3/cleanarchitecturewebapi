using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Pais.Commands.ImportarDatos
{
    public partial class ImportarDatosCommandHandler : IRequestHandler<ImportarDatosCommand, ErrorOr<Created>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ImportarDatosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Created>> Handle(ImportarDatosCommand request, CancellationToken cancellationToken)
        {
            var duplicadosArchivo = ExistenDuplicados(request.Datos);
            switch (duplicadosArchivo){
                case 1: return ErroresPaises.NombreDuplicado;
                case 2: return ErroresPaises.CodigoA2Duplicado;
                case 3: return ErroresPaises.CodigoNumericoDuplicado;
                case 4: return ErroresPaises.CodigoC3Duplicado;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var datosExistentes = await _unitOfWork.PaisesRepository.ObtenerPaises();

                if (datosExistentes.IsError)
                {
                    return datosExistentes.Errors;
                }

                foreach (var datoExistente in datosExistentes.Value)
                {
                    var resultadoDelete = await _unitOfWork.PaisesRepository.EliminarPais(datoExistente.Id);

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
                    var existe = await _unitOfWork.PaisesRepository.ValidarPais(datoInsertar.Id, datoInsertar.Nombre,datoInsertar.CodigoA2,datoInsertar.CodigoNumerico,datoInsertar.CodigoC3);

                    if (existe.Value is not null)
                    {
                        if (Validadores.NormalizarString(existe.Value.Nombre) == Validadores.NormalizarString(datoInsertar.Nombre))
                        {
                            return ErroresPaises.NombreDuplicado;
                        }

                        if (existe.Value.CodigoA2 == datoInsertar.CodigoA2)
                        {
                            return ErroresPaises.CodigoA2Duplicado;
                        }

                        if (existe.Value.CodigoC3 == datoInsertar.CodigoC3)
                        {
                            return ErroresPaises.CodigoC3Duplicado;
                        }

                        if (existe.Value.CodigoNumerico == datoInsertar.CodigoNumerico)
                        {
                            return ErroresPaises.CodigoNumericoDuplicado;
                        }
                    }
                }

                if (Validadores.Nombre(datoInsertar.Nombre))
                {
                    return ErroresPaises.ValidacionNombre;
                }

                if (!Validadores.IsValidoCodigoA2(datoInsertar.CodigoA2))
                {
                    return ErroresPaises.CodigoA2ExcedeLimite;
                }
                if (!Validadores.IsValidoCodigoNumerico(datoInsertar.CodigoNumerico))
                {
                    return ErroresPaises.CodigoNumericoExcedeLimite;
                }
                if (!Validadores.IsValidoCodigoC3(datoInsertar.CodigoC3))
                {
                    return ErroresPaises.CodigoC3ExcedeLimite;
                }

                var resultInsertar = await _unitOfWork.PaisesRepository.CrearPais(datoInsertar);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private static int ExistenDuplicados(IEnumerable<Dominio.Entidades.Pais> datos)
        {
            HashSet<string> datosComprobadosNombre = new HashSet<string>();
            HashSet<string> datosComprobadosCodigoA2 = new HashSet<string>();
            HashSet<string> datosComprobadosCodigoNumerico = new HashSet<string>();
            HashSet<string> datosComprobadosCodigoC3 = new HashSet<string>();


            foreach (var pais in datos)
            {
                if (!datosComprobadosNombre.Add(Validadores.NormalizarString(pais.Nombre)))
                {
                    return 1;
                }
                if (!datosComprobadosCodigoA2.Add(pais.CodigoA2))
                {
                    return 2;
                }
                if (!datosComprobadosCodigoNumerico.Add(pais.CodigoNumerico))
                {
                    return 3;
                }
                if (!datosComprobadosCodigoC3.Add(pais.CodigoC3))
                {
                    return 4;
                }
            }

            return 0;
        }

    }
}
