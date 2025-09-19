using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IFamiliaRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.Familia>>> ObtenerFamilias();
        public Task<ErrorOr<Dominio.Entidades.Familia>> ObtenerFamiliaPorId(int Id);

        public Task<ErrorOr<Dominio.Entidades.Familia>> CrearFamilia(Dominio.Entidades.Familia familia);

        public Task<ErrorOr<Dominio.Entidades.Familia>> ActualizarFamilia(int idFamilia, List<string> listaCambios, Dominio.Entidades.Familia familia);

        public Task<ErrorOr<Deleted>> EliminarFamilia(int id);

        public Task<ErrorOr<bool>> ValidarFamilia(int idFamilia, string nombre);
    }
}
