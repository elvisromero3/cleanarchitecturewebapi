using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogos
{
    public class ObtenerCatalogosQuery : IRequest<ErrorOr<List<Dominio.Entidades.Catalogo>>>
    {
    }
}
