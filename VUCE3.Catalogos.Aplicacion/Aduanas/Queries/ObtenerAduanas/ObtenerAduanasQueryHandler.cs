using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Queries.ObtenerAduanas
{
    public class ObtenerAduanasQueryHandler : IRequestHandler<ObtenerAduanasQuery, ErrorOr<List<Aduana>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ObtenerAduanasQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Aduana>>> Handle(ObtenerAduanasQuery request, CancellationToken cancellationToken)
        {
            var aduanas = await _unitOfWork.AduanasRepository.ObtenerAduanas();
            return aduanas;
        }
    }
}
