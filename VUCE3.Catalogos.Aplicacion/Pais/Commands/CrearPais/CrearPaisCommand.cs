using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Pais.Commands.CrearPais
{
    public class CrearPaisCommand : IRequest<ErrorOr<Dominio.Entidades.Pais>>
    {
        public Dominio.Entidades.Pais Pais { get; set; } = null!;
    }
}
