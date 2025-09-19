using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Variedad.Commands.EliminarVariedades
{
    public class EliminarVariedadesCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsVariedades { get; set; } = null!;
    }
}
