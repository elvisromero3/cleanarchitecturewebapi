using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Casa.Commands.EliminarCasas
{
    public class EliminarCasasCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsCasas { get; set; } = null!;
    }
}
