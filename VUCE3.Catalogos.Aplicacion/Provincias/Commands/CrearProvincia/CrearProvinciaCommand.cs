using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Provincia.Commands.CrearProvincia
{
    public class CrearProvinciaCommand : IRequest<ErrorOr<Dominio.Entidades.Provincia>>
    {
        public Dominio.Entidades.Provincia Provincia { get; set; } = null!;
    }
}
