using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;


namespace VUCE3.Catalogos.Aplicacion.TipoProducto.Queries.ObtenerTipoProductoPorId
{
    public class ObtenerTipoProductoPorIdQueryHandler : IRequestHandler<ObtenerTipoProductoPorIdQuery, ErrorOr<Dominio.Entidades.TipoProducto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerTipoProductoPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.TipoProducto>> Handle(ObtenerTipoProductoPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TipoProductoRepository.ObtenerTipoProductoPorId(request.IdTipoProducto);
        }
    
    }
}
