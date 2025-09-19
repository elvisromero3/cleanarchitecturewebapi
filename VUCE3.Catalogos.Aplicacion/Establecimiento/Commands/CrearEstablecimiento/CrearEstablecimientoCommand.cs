using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.CrearEstablecimiento
{
    public class CrearEstablecimientoCommand : IRequest<ErrorOr<Dominio.Entidades.Establecimiento>>
    {
        public Dominio.Entidades.Establecimiento Establecimiento { get; set; } = null!;
    }
}
