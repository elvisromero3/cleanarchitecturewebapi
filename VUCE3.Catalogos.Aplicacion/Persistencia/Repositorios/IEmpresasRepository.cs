using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IEmpresasRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.Empresa>>> ObtenerEmpresas();
        public Task<ErrorOr<List<Dominio.Entidades.Empresa>>> ObtenerEmpresasPorIdProfesional(int id);
        public Task<ErrorOr<Dominio.Entidades.Empresa>> ObtenerEmpresaPorId(int id);        
        public Task<ErrorOr<Dominio.Entidades.Empresa>> CrearEmpresa(Dominio.Entidades.Empresa empresa);
        public Task<ErrorOr<Dominio.Entidades.Empresa>> ActualizarEmpresa(Dominio.Entidades.Empresa empresa, int idEmpresa, IEnumerable<string> changeList);
        public Task<ErrorOr<Deleted>> EliminarEmpresa(int id);
        public Task<ErrorOr<bool>> ValidarEmpresa(int idEmpresa, char idTipoIdentificacion, string numeroIdentificacion, int idProfesional);

    }
}
