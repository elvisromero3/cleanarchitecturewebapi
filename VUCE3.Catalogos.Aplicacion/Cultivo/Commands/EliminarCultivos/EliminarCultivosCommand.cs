using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EliminarCultivos
{
    public class EliminarCultivosCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsCultivos { get; set; } = null!;
    }
}
