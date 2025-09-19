using ErrorOr;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IAduanasRepository
    {
        public Task<ErrorOr<List<Aduana>>> ObtenerAduanas();
        public Task<ErrorOr<Aduana>> ObtenerAduanaPorId(int Id);
        public Task<ErrorOr<Aduana>> CrearAduana(Aduana aduana);
        public Task<ErrorOr<Aduana>> ActualizarAduana(Aduana aduana, int idAduana, IEnumerable<string> changeList);
        public Task<ErrorOr<Deleted>> EliminarAduana(int id);
        public Task<ErrorOr<bool>> ValidarAduana(Aduana aduana);

    }
}
