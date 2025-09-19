using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;


namespace VUCE3.Catalogos.Aplicacion.BloqueComercial.Queries.ObtenerBloqueComercialPorId
{
    public class ObtenerBloqueComercialPorIdQueryHandler : IRequestHandler<ObtenerBloqueComercialPorIdQuery, ErrorOr<Dominio.Entidades.BloqueComercial>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerBloqueComercialPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.BloqueComercial>> Handle(ObtenerBloqueComercialPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.BloqueComercialRepository.ObtenerBloqueComercialPorId(request.IdBloqueComercial);
        }
    
    }
}
