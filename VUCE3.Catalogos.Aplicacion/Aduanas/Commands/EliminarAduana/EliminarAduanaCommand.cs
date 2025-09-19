using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EliminaAduana
{
    public class EliminarAduanaCommand : IRequest<ErrorOr<Aduana>>
    {
        public int Id { get; set; }
    }
}
