    using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;


namespace VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.EliminarTipoProducto
{
    public class EliminarTipoProductoCommandHandler : IRequestHandler<EliminarTipoProductoCommand, ErrorOr<Dominio.Entidades.TipoProducto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarTipoProductoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.TipoProducto>> Handle(EliminarTipoProductoCommand command, CancellationToken cancellationToken)
        {
            var tipoProducto = await _unitOfWork.TipoProductoRepository.ObtenerTipoProductoPorId(command.Id);

            if (tipoProducto.IsError)
            {
                return tipoProducto.Errors;
            }

            var productorequisitoexite = await _unitOfWork.TipoProductoRepository.ValidarExisteProductoRequisito(command.Id);
            if (productorequisitoexite.Value)
            {
                return ErroresTipoProducto.ProductoRequisitoExiste;
            }

                var result = await _unitOfWork.TipoProductoRepository.EliminarTipoProducto(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return tipoProducto;
        }
    }
}

