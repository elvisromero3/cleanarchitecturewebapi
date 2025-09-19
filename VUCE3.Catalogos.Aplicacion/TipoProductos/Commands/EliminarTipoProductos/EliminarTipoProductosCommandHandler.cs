using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.EliminarTipoProductos;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.EliminarTipoProductos
{
    public class EliminarTipoProductosCommandHandler : IRequestHandler<EliminarTipoProductosCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarTipoProductosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarTipoProductosCommand request, CancellationToken cancellationToken)
        {
            foreach (var idTipoProducto in request.IdsTipoProductos)
            {
                var productorequisitoexite = await _unitOfWork.TipoProductoRepository.ValidarExisteProductoRequisito(idTipoProducto);
                if (productorequisitoexite.Value)
                {
                    return ErroresTipoProducto.ProductoRequisitoExiste;
                }

                var result = await _unitOfWork.TipoProductoRepository.EliminarTipoProducto(idTipoProducto);

                if (result.IsError)
                {
                    return result.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Deleted;
        }
    }
}
