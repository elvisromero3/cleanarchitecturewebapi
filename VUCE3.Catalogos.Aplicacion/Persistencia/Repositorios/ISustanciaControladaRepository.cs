using ErrorOr;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface ISustanciaControladaRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.SustanciaControlada>>> ObtenerSustanciaControladas();
        public Task<ErrorOr<Dominio.Entidades.SustanciaControlada>> ObtenerSustanciaControladaPorId(int Id);

        public Task<ErrorOr<Dominio.Entidades.SustanciaControlada>> CrearSustanciaControlada(Dominio.Entidades.SustanciaControlada sustanciaControlada);

        public Task<ErrorOr<Dominio.Entidades.SustanciaControlada>> ActualizarSustanciaControlada(int idSustanciaControlada, List<string> listaCambios, Dominio.Entidades.SustanciaControlada sustanciaControlada);

        public Task<ErrorOr<Deleted>> EliminarSustanciaControlada(int id);

        public Task<ErrorOr<bool>> ValidarSustanciasControladas(int id, Dominio.Entidades.SustanciaControlada sustanciaControlada);

        public Task<ErrorOr<Dominio.Entidades.Familia>> ObtenerFamiliaSustanciaControlada(int idFamilia);
    }
}
