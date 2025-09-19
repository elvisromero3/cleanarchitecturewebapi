using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Queries.ObtenerAduanaPorId
{
    public class ObtenerAduanaPorIdQuery : IRequest<ErrorOr<Aduana>>
    {
        public int Id { get; set; }
    }
}
