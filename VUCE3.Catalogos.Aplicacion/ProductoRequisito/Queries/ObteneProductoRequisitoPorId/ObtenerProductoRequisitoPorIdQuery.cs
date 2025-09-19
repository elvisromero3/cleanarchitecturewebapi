using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.ProductoRequisito.Queries.ObtenerProductoRequisitosPorId
{
    public class ObtenerProductoRequisitoPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.ProductoRequisito>>
    {
        public int IdProductoRequisito { get; set; }
    }
}
