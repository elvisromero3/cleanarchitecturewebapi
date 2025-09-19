using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Sustancia.Commands.CrearSustancia
{
    public class CrearSustanciaCommand : IRequest<ErrorOr<Dominio.Entidades.Sustancia>>
    {
        public Dominio.Entidades.Sustancia Sustancia { get; set; } = null!;
    }
}
