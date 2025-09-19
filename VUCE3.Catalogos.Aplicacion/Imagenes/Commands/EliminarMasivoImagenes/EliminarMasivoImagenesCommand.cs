using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Imagenes.Commands.EliminarMasivoImagenes
{
    public class EliminarMasivoImagenesCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsImagenes { get; set; } = null!;
    }
}
