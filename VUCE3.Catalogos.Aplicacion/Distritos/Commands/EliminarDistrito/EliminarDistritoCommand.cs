using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Commands.EliminarDistrito
{
    public class EliminarDistritoCommand : IRequest<ErrorOr<Distrito>>
    {
        public int Id { get; set; }
    }
}
