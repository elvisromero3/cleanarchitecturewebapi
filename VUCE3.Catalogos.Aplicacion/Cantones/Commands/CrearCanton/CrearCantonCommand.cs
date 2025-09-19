using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Cantones.Commands.CrearCanton
{
    public class CrearCantonCommand : IRequest<ErrorOr<Dominio.Entidades.Canton>>
    {
        public Dominio.Entidades.Canton Canton { get; set; } = null!;
    }
}
