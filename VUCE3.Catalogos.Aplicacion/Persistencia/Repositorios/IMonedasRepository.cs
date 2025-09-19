using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IMonedasRepository
    {
        Task<ErrorOr<List<Dominio.Entidades.Moneda>>> ObtenerMonedas();
        Task<ErrorOr<Dominio.Entidades.Moneda>> ObtenerMonedaPorId(int id);
    }
}
