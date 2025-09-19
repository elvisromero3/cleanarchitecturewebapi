using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Pais.Commands.EliminarPaises
{
    public class EliminarPaisesCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsPaises { get; set; } = null!;
    }
}
