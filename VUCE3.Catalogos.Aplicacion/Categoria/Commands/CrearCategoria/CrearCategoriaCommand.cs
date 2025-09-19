using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Categoria.Commands.CrearCategoria
{
    public class CrearCategoriaCommand : IRequest<ErrorOr<Dominio.Entidades.Categoria>>
    {
        public Dominio.Entidades.Categoria Categoria { get; set; } = null!;
    }
}
