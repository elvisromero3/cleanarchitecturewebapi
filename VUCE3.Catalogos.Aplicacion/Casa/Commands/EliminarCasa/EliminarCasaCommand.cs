using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Casa.Commands.EliminarCasa
{
    public class EliminarCasaCommand : IRequest<ErrorOr<Dominio.Entidades.Casa>>
    {
        public int Id { get; set; }
    }
}
