using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EliminarSustancia
{
    public class EliminarSustanciaCommand : IRequest<ErrorOr<Dominio.Entidades.Sustancia>>
    {
        public int IdSustancia { get; set; }
    }
}
