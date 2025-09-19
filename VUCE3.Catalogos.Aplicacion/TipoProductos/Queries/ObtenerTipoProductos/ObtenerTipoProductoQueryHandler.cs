using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.TipoProducto.Queries.ObtenerTipoProductos
{
    public class ObtenerTipoProductosQueryHandler : IRequestHandler<ObtenerTipoProductosQuery, ErrorOr<List<Dominio.Entidades.TipoProducto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerTipoProductosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.TipoProducto>>> Handle(ObtenerTipoProductosQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TipoProductoRepository.ObtenerTipoProductos();
        }
    }
}
