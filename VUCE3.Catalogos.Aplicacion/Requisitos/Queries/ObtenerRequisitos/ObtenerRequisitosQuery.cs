using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Requisitos.Queries.ObtenerRequisitos
{
    public class ObtenerRequisitosQuery : IRequest<ErrorOr<List<Requisito>>>
    {
    }
}
