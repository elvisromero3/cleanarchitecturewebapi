using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Categoria.Commands.EditarCategoria
{
    public class EditarCategoriaCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Categoria, Dominio.Entidades.Categoria>>>
    {
        public Dominio.Entidades.Categoria Categoria { get; set; } = null!;
        public required int IdCategoria { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
