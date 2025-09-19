using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Variedad.Commands.EliminarVariedad
{
    public class EliminarVariedadCommand : IRequest<ErrorOr<Dominio.Entidades.Variedad>>
    {
        public int IdVariedad { get; set; }
    }
}
