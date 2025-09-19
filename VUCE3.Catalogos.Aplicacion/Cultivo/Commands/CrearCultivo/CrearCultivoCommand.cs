using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Commands.CrearCultivo
{
    public class CrearCultivoCommand : IRequest<ErrorOr<Dominio.Entidades.Cultivo>>
    {
        public Dominio.Entidades.Cultivo Cultivo { get; set; } = null!;
    }
}
