using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.ProductoRequisito.Queries.ObtenerProductoRequisitos
{
    public class ObtenerProductoRequisitoQuery : IRequest<ErrorOr<List<Dominio.Entidades.ProductoRequisito>>>
    {
    }
}
