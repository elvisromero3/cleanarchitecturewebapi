using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Profesional.Commands.EliminarProfesional
{
    public class EliminarProfesionalCommand : IRequest<ErrorOr<Dominio.Entidades.Profesional>>
    {
        public int Id { get; set; }
    }
}
