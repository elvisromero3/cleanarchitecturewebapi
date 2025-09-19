using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Familia.Commands.EliminarFamilia
{
    public class EliminarFamiliaCommand : IRequest<ErrorOr<Dominio.Entidades.Familia>>
    {
        public int Id { get; set; }
    }
}
