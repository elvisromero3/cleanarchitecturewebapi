using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.TipoProducto.Queries.ObtenerTipoProductoPorId
{
    public class ObtenerTipoProductoPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.TipoProducto>>
    {
        public int IdTipoProducto { get; set; }
    
    }
}
