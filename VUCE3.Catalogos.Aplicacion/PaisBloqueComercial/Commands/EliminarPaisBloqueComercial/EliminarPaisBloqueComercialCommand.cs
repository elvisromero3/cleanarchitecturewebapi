using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;
namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EliminarPaisBloqueComercial
{
    public class EliminarPaisBloqueComercialCommand : IRequest<ErrorOr<Dominio.Entidades.PaisBloqueComercial>>
    {
        public int Id { get; set; }
    }
}
