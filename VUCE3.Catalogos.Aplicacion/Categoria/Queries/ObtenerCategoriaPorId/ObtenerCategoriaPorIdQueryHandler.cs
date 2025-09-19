using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Categoria.Queries.ObtenerCategoriaPorId
{
    public class ObtenerCategoriaPorIdQueryHandler : IRequestHandler<ObtenerCategoriaPorIdQuery, ErrorOr<Dominio.Entidades.Categoria>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCategoriaPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Categoria>> Handle(ObtenerCategoriaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CategoriaRepository.ObtenerCategoriaPorId(request.IdCategoria);
        }
    }
}
