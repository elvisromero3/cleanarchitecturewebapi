using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Productos.Commands.EliminarProductos
{
    public class EliminarProductosCommandHandler : IRequestHandler<EliminarProductosCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarProductosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarProductosCommand request, CancellationToken cancellationToken)
        {
            foreach (var id in request.Ids)
            {
                var productos = await _unitOfWork.ProductosRepository.ObtenerProductoPorId(id);
                if (productos.IsError)
                {
                    return productos.Errors;
                }
            }

            foreach (var id in request.Ids)
            {
                var result = await _unitOfWork.ProductosRepository.EliminarProductos(id);

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
