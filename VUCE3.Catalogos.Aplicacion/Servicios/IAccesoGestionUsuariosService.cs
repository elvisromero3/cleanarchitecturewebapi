using ErrorOr;
using VUCE3.Catalogos.Aplicacion.Servicios.DTO;

namespace VUCE3.Catalogos.Aplicacion.Servicios
{
    public interface IAccesoGestionUsuariosService
    {
        public Task<ErrorOr<List<InstitucionDto>>> ObtenerInstituciones();
        public Task<ErrorOr<bool>> ExisteRelacionAduanaInstitucionesAutorizadas(int idAduana);
    }
}
