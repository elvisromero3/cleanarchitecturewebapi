using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Commands.EliminarEmpresas
{
    public class EliminarEmpresasCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsEmpresas { get; set; } = null!;
    }
}
