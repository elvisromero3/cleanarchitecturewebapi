using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EliminarAduanas
{
    public class EliminarAduanasCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsAduanas { get; set; } = null!;
    }
}
