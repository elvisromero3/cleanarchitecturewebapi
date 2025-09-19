using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Queries.ObtenerBarrios
{
    public class ObtenerBarriosQueryHandler : IRequestHandler<ObtenerBarriosQuery, ErrorOr<List<Barrio>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ObtenerBarriosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Barrio>>> Handle(ObtenerBarriosQuery request, CancellationToken cancellationToken)
        {
            var barrios = await _unitOfWork.BarriosRepository.ObtenerBarrios();
            return barrios;
        }
    }
}
