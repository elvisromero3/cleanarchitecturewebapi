using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IEstablecimientosRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.Establecimiento>>> ObtenerEstablecimientos();
        public Task<ErrorOr<Dominio.Entidades.Establecimiento>> ObtenerEstablecimientoPorId(int Id);

        public Task<ErrorOr<Dominio.Entidades.Establecimiento>> CrearEstablecimiento(Dominio.Entidades.Establecimiento casa);

        public Task<ErrorOr<Dominio.Entidades.Establecimiento>> ActualizarEstablecimiento(int id, List<string> listaCambios, Dominio.Entidades.Establecimiento casa);

        public Task<ErrorOr<Deleted>> EliminarEstablecimiento(int id);

        public Task<ErrorOr<bool>> ValidarEstablecimiento(int id, string numeroCvo);
    }
}
