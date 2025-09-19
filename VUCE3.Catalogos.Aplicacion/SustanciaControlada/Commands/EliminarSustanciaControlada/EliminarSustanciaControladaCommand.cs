using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.EliminarSustanciaControlada
{
    public class EliminarSustanciaControladaCommand : IRequest<ErrorOr<Dominio.Entidades.SustanciaControlada>>
    {
        public int Id { get; set; }
    }
}
