using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Requisitos.Queries.ObtenerRequisitoPorId
{
    public class ObtenerRequisitoPorIdQuery : IRequest<ErrorOr<Requisito>>
    {
        public int Id { get; set; }
    }
}
