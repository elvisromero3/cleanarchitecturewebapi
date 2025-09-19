using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.CrearProductoRequisito
{
    public class CrearProductoRequisitoCommand : IRequest<ErrorOr<Dominio.Entidades.ProductoRequisito>>
    {
        public Dominio.Entidades.ProductoRequisito ProductoRequisito { get; set; } = null!;
    }
}
