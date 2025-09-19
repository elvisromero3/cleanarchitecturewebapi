using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogoPorId
{
    public class ObtenerCatalogoPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Catalogo>>
    {
        public int IdCatalogo { get; set; }
    }
}
