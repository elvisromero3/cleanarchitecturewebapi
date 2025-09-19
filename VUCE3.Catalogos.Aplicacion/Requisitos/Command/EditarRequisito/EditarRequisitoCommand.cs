using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Requisitos.Command.EditarRequisito
{
    public class EditarRequisitoCommand : IRequest<ErrorOr<Tuple<Requisito, Requisito>>>
    {
        public Requisito Requisito { get; set; } = null!;
        public required int IdRequisito { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
