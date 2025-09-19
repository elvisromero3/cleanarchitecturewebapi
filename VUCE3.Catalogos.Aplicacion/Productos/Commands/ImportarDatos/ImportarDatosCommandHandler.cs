using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Productos.Commands.ImportarDatos
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
                return ErroresProductos.DatosDuplicadosArchivo;
            }

            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var resultadoDelete = await eliminarProductos();
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            foreach (var datoInsertar in request.Datos)
            {
                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var sector = await _unitOfWork.ProductosRepository.ValidarProductos(datoInsertar.Id, datoInsertar.NombreComun, datoInsertar.NombreCientifico);
                    if (sector.Value)
                    {
                        return ErroresProductos.DatosDuplicados;
                    }
                }

                if (Validadores.Nombre50(datoInsertar.NombreComun))
                {
                    return ErroresProductos.NombreComunInvalido;
                }

                if (Validadores.Nombre50(datoInsertar.NombreCientifico))
                {
                    return ErroresProductos.TamanoNombreCientifico;
                }

                if (Validadores.Codigo20(datoInsertar.Clase))
                {
                    return ErroresProductos.TamanoClase;
                }

                if (Validadores.Codigo20(datoInsertar.Presentacion))
                {
                    return ErroresProductos.TamanoPresentacion;
                }

                var resultInsertar = await _unitOfWork.ProductosRepository.CrearProductos(datoInsertar);
                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }

        private async Task<ErrorOr<Success>> eliminarProductos()
        {
            var datosExistentes = await _unitOfWork.ProductosRepository.ObtenerProductos();
            if (datosExistentes.IsError)
            {
                return datosExistentes.Errors;
            }

            foreach (var sector in datosExistentes.Value)
            {
                var resultadoDelete = await _unitOfWork.ProductosRepository.EliminarProductos(sector.Id);
                if (resultadoDelete.IsError)
                {
                    return resultadoDelete.Errors;
                }
            }

            return Result.Success;
        }

        private bool ExistenDuplicados(IEnumerable<Dominio.Entidades.Productos> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var producto in datos)
            {
                if (!datosComprobados.Add(Validadores.NormalizarString(producto.NombreComun)+"_"+ Validadores.NormalizarString(producto.NombreCientifico)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
