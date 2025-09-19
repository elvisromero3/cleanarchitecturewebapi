using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Provincia.Commands.EliminarProvincia
{
    public class EliminarProvinciaCommand : IRequest<ErrorOr<Dominio.Entidades.Provincia>>
    {
        public int Id { get; set; }
    }
}
