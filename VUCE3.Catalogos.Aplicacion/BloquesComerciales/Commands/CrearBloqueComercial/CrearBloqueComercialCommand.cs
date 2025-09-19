using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.CrearBloqueComercial
{
    public class CrearBloqueComercialCommand : IRequest<ErrorOr<Dominio.Entidades.BloqueComercial>>
    {
        public Dominio.Entidades.BloqueComercial BloqueComercial { get; set; } = null!;
    }
}
