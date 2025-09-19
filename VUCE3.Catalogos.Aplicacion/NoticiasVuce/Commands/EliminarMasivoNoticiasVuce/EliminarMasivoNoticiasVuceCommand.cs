using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EliminarMasivoNoticiasVuce
{
    public class EliminarMasivoNoticiasVuceCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsNoticias { get; set; } = null!;
    }
}
