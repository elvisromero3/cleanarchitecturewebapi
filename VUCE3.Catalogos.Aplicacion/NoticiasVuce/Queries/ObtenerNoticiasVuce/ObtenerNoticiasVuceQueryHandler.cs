using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Pais.Queries.ObtenerPaises;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.NoticiasVuce.Queries.ObtenerNoticiasVuce
{
    public class ObtenerNoticiasVuceQueryHandler : IRequestHandler<ObtenerNoticiasVuceQuery, ErrorOr<List<Dominio.Entidades.NoticiasVuce>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerNoticiasVuceQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.NoticiasVuce>>> Handle(ObtenerNoticiasVuceQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.NoticiasVuceRepository.ObtenerNoticiasVuce();
        }
    }
}