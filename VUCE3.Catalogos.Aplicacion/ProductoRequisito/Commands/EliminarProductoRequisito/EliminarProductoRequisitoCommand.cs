using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EliminarProductoRequisito
{
    public class EliminarProductoRequisitoCommand : IRequest<ErrorOr<Dominio.Entidades.ProductoRequisito>>
    {
        public int Id { get; set; }
    }
}
