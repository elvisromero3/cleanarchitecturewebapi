using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Categoria.Queries.ObtenerCategorias
{
    public class ObtenerCategoriasQueryHandler : IRequestHandler<ObtenerCategoriasQuery, ErrorOr<List<Dominio.Entidades.Categoria>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCategoriasQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Categoria>>> Handle(ObtenerCategoriasQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CategoriaRepository.ObtenerCategorias();
        }
    }
}
