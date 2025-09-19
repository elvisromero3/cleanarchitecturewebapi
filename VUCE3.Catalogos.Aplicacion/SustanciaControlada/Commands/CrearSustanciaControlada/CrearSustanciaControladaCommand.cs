using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.CrearSustanciaControlada
{
    public class CrearSustanciaControladaCommand : IRequest<ErrorOr<Dominio.Entidades.SustanciaControlada>>
    {
        public Dominio.Entidades.SustanciaControlada SustanciaControlada { get; set; } = null!;
    }
}
