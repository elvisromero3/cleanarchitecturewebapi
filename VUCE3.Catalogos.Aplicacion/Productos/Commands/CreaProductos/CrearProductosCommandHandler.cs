using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Productos.Commands.CrearProductos
{
    public class CrearProductosCommandHandler : IRequestHandler<CrearProductosCommand, ErrorOr<Dominio.Entidades.Productos>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearProductosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Productos>> Handle(CrearProductosCommand request, CancellationToken cancellationToken)
        {
            //Verifica el tamaño de Nombre o si es vacío              
            if (Validadores.Nombre50(request.Productos.NombreComun))
            {
                return ErroresProductos.NombreComunInvalido;
            }

            if (Validadores.Codigo20(request.Productos.Clase))
            {
                return ErroresProductos.TamanoClase;
            }

            if (Validadores.Codigo20(request.Productos.Presentacion))
            {
                return ErroresProductos.TamanoPresentacion;
            }

            if (Validadores.Nombre50(request.Productos.NombreCientifico))
            {
                return ErroresProductos.TamanoNombreCientifico;
            }

            var existe = await _unitOfWork.ProductosRepository.ValidarProductos(
                request.Productos.Id, request.Productos.NombreComun, request.Productos.NombreCientifico);
            if (existe.Value)
            {
                return ErroresProductos.DatosDuplicados;
            }

            var result = await _unitOfWork.ProductosRepository.CrearProductos(request.Productos);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }

    }
}
