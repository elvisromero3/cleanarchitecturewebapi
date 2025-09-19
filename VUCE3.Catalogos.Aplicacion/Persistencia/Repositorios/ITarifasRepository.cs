using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface ITarifasRepository
    {
        Task<ErrorOr<List<Dominio.Entidades.Tarifa>>> ObtenerTarifas();
        Task<ErrorOr<Dominio.Entidades.Tarifa>> ObtenerTarifaPorId(int id);
    }
}
