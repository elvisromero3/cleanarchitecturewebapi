using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Pais.Commands.EliminarPais
{
    public class EliminarPaisCommand : IRequest<ErrorOr<Dominio.Entidades.Pais>>
    {
        public int Id { get; set; }
    }
}
