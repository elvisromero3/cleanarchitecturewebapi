using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Casa.Commands.CrearCasa
{
    public class CrearCasaCommand : IRequest<ErrorOr<Dominio.Entidades.Casa>>
    {
        public Dominio.Entidades.Casa Casa { get; set; } = null!;
    }
}
