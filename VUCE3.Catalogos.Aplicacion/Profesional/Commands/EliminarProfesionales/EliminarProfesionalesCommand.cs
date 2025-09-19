using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Profesional.Commands.EliminarProfesionales
{
    public class EliminarProfesionalesCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsProfesionales { get; set; } = null!;
    }
}
