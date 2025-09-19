using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.EliminarTipoProductos
{
    public class EliminarTipoProductosCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsTipoProductos { get; set; } = null!;
    }
}
