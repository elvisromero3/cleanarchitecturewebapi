using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EliminarEstablecimiento
{
    public class EliminarEstablecimientoCommand : IRequest<ErrorOr<Dominio.Entidades.Establecimiento>>
    {
        public int Id { get; set; }
    }
}
