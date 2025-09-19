using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EliminarCultivo
{
    public class EliminarCultivoCommand : IRequest<ErrorOr<Dominio.Entidades.Cultivo>>
    {
        public int IdCultivo { get; set; }
    }
}
