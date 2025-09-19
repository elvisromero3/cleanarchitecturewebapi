using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Commands.EliminarEmpresa
{
    public class EliminarEmpresaCommand : IRequest<ErrorOr<Dominio.Entidades.Empresa>>
    {
        public int Id { get; set; }
    }
}
