using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EditarProductoRequisito
{
    public class EditarProductoRequisitoCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.ProductoRequisito, Dominio.Entidades.ProductoRequisito>>>
    {
        public Dominio.Entidades.ProductoRequisito ProductoRequisito { get; set; } = null!;
        public required int IdProductoRequisito { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
