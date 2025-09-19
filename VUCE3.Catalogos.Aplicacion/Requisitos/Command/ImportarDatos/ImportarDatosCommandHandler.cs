using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos
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
                return ErroresRequisito.RequisitoDatosDuplicadosArchivo;
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

        private static bool ExistenDuplicados(IEnumerable<ImportarRequisitoCommandDto> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var requisito in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(requisito.Descripcion)))
                {
                    return true;
                }
            }

            return false;
        }
        private async Task<ErrorOr<Created>> EliminarDatosExistentes()
        {
            var datosExistentes = await _unitOfWork.RequisitosRepository.ObtenerRequisitos();

            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            foreach (var datoExistente in datosExistentes.Value)
            {
                var resultadoDelete = await _unitOfWork.RequisitosRepository.EliminarRequisito(datoExistente.Id);

                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Created;
        }
        private async Task<ErrorOr<Created>> InsertarDatos(ImportarDatosCommand request)
        {
            var paises = await _unitOfWork.PaisesRepository.ObtenerPaises();

            foreach (var datoInsertar in request.Datos)
            {
                //Verifica tamaño de descripción o si es vacío 
                if (Validadores.LongitudMaximaNoNull(datoInsertar.Descripcion, 300))
                {
                    return ErroresRequisito.RequisitoDescripcionInvalida;
                }
                //Verifica tamaño de Codigo
                if (Validadores.LongitudMaximaNoNull(datoInsertar.Codigo, 50))
                {
                    return ErroresRequisito.RequisitoCodigoTamano;
                }

                //Verifica tamaño de Version
                if (Validadores.LongitudMaximaNoNull(datoInsertar.Version, 10))
                {
                    return ErroresRequisito.RequisitoVersionTamano;
                }
             

                var pais = paises.Value.Find(x => x.Nombre == datoInsertar.Pais);
                if (pais is null)
                {
                    return ErroresRequisito.RequisitoPaisNoEncontrado;
                }

                //Verifica la institucion
                if (Validadores.institucionDCAoDIPOA(datoInsertar.IdInstitucion))
                {
                    return ErroresRequisito.RequisitoInstitucionInvalida;
                }

                var requisito = new Dominio.Entidades.Requisito { 
                    Codigo = datoInsertar.Codigo, 
                    Descripcion = datoInsertar.Descripcion, 
                    Version = datoInsertar.Version,
                    IdPais = pais.Id,
                    IdInstitucion = datoInsertar.IdInstitucion,
                    Activo = true,
                 };

                var resultInsertar = await _unitOfWork.RequisitosRepository.CrearRequisito(requisito);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            return Result.Created;
        }
    }
}