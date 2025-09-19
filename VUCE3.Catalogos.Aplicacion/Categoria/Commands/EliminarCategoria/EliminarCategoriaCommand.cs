using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Categoria.Commands.EliminarCategoria
{
    public class EliminarCategoriaCommand : IRequest<ErrorOr<Dominio.Entidades.Categoria>>
    {
        public int Id { get; set; }
    }
}
