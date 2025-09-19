using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Commands.EliminarSectores
{
    public class EliminarSectoresCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsSectores { get; set; } = null!;
    }
}
