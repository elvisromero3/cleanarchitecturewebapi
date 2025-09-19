using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.EliminarBloqueComercial
{
    public class EliminarBloqueComercialCommand : IRequest<ErrorOr<Dominio.Entidades.BloqueComercial>>
    {
        public int Id { get; set; }
    }
}
