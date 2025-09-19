using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Queries.ObtenerEmpresas
{
    public class ObtenerEmpresasQuery : IRequest<ErrorOr<List<Dominio.Entidades.Empresa>>>
    {
    }
}
