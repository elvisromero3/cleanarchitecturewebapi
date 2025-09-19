using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisito
{
    public class EliminarRequisitoCommand : IRequest<ErrorOr<Requisito>>
    {
        public int Id { get; set; }
    }
}
