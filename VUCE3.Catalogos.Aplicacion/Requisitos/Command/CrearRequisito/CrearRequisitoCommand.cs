using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Requisitos.Command.CrearRequisito
{
    public class CrearRequisitoCommand : IRequest<ErrorOr<Requisito>>
    {
        public Requisito Requisito { get; set; } = null!;
    }
}
