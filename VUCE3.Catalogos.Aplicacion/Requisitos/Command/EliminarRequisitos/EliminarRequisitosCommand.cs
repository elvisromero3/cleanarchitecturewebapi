using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisitos
{
    public class EliminarRequisitosCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsRequisitos { get; set; } = null!;
    }
}
