using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Categoria.Commands.EliminarCategoria
{
    public class EliminarCategoriaCommandHandler : IRequestHandler<EliminarCategoriaCommand, ErrorOr<Dominio.Entidades.Categoria>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarCategoriaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Categoria>> Handle(EliminarCategoriaCommand command, CancellationToken cancellationToken)
        {
            var Categoria = await _unitOfWork.CategoriaRepository.ObtenerCategoriaPorId(command.Id);

            if (Categoria.IsError)
            {
                return Categoria.Errors;
            }
            
            var tipoProductos = await _unitOfWork.TipoProductoRepository.ObtenerTipoProductos();
            var tipoProducto = tipoProductos.Value.Count(x => x.IdCategoria == command.Id);
            if (tipoProducto > 0)
            {
                return ErroresCategoria.TipoProductoRelacionado;
            }

            var result = await _unitOfWork.CategoriaRepository.EliminarCategoria(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Categoria;
        }
    }
}
