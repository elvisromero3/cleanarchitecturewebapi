using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Servicios
{
    public interface ITramitesService
    {
        public Task<ErrorOr<bool>> ExisteRelacionTipoTramiteSubtipo(int idTipoTramite, int idSubtipoTramite);
    }
}
