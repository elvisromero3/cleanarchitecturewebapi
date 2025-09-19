using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Clientes.Commands.EliminarClientes
{
    public class EliminarClientesCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsClientes { get; set; } = null!;
    }
}
