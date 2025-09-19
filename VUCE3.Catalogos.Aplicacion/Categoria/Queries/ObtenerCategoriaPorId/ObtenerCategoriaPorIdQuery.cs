using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Categoria.Queries.ObtenerCategoriaPorId
{
    public class ObtenerCategoriaPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Categoria>>
    {
        public int IdCategoria { get; set; }

    }
}
