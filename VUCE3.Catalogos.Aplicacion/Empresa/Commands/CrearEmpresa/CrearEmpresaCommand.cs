using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Commands.CrearEmpresa
{
    public class CrearEmpresaCommand : IRequest<ErrorOr<Dominio.Entidades.Empresa>>
    {
        public Dominio.Entidades.Empresa Empresa { get; set; } = null!;
    }
}
