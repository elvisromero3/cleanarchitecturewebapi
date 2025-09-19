using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Productos.Commands.EditarProductos;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Productos.Commands.EditarProductos
{
    public class EditarProductosCommandHandler : IRequestHandler<EditarProductosCommand, ErrorOr<Tuple<Dominio.Entidades.Productos, Dominio.Entidades.Productos>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarProductosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.Productos, Dominio.Entidades.Productos>>> Handle(EditarProductosCommand command, CancellationToken cancellationToken)
        {
            // Obtener Productos por Id
            var productos = await _unitOfWork.ProductosRepository.ObtenerProductoPorId(command.IdProducto);
            if (productos.IsError)
            {
                return productos.Errors;
            }

            var comprobarNombreComun = productos.Value.NombreComun;
            var comprobarNombreCientifico = productos.Value.NombreCientifico;
            var comprobarDuplicado = false;
                       
            if (command.ListaCambios.Contains("Clase") && Validadores.Codigo20(command.Productos.Clase))
            {
                return ErroresProductos.TamanoClase;
            }

            if (command.ListaCambios.Contains("Presentacion") && Validadores.Codigo20(command.Productos.Presentacion))
            {
                return ErroresProductos.TamanoPresentacion;
            }

            if (command.ListaCambios.Contains("NombreComun"))
            {
                if (Validadores.Nombre50(command.Productos.NombreComun))
                {
                    return ErroresProductos.NombreComunInvalido;
                }
                comprobarNombreComun = command.Productos.NombreComun;
                comprobarDuplicado = true;
            }

            if (command.ListaCambios.Contains("NombreCientifico"))
            {
                if (Validadores.Nombre50(command.Productos.NombreCientifico))
                {
                    return ErroresProductos.TamanoNombreCientifico;
                }
                comprobarNombreCientifico = command.Productos.NombreCientifico;
                comprobarDuplicado = true;
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.ProductosRepository.ValidarProductos(command.IdProducto, comprobarNombreComun, comprobarNombreCientifico);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresProductos.DatosDuplicados;
                }
            }

            //se obtiene copia de la Productos antes de aplicar cambios
            Dominio.Entidades.Productos productosAntes = JsonConvert.DeserializeObject<Dominio.Entidades.Productos>(
                JsonConvert.SerializeObject(productos.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? productos.Value;

            var result = await _unitOfWork.ProductosRepository.ActualizarProductos(command.IdProducto, command.ListaCambios, command.Productos);
            if (result.IsError)
            {
                return result.Errors;
            }
            await _unitOfWork.Save();

            return Tuple.Create(productosAntes, productos.Value);
        }
    }
}