using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Queries.ObtenerDistritos
{
    public class ObtenerDistritosQueryHandler : IRequestHandler<ObtenerDistritosQuery, ErrorOr<List<Distrito>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ObtenerDistritosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Distrito>>> Handle(ObtenerDistritosQuery request, CancellationToken cancellationToken)
        {
            var distritos = await _unitOfWork.DistritosRepository.ObtenerDistritos();
            return distritos;
        }
    }
}
