using ErrorOr;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Aplicacion.Servicios.DTO;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Servicios
{
    public class TramitesService : ITramitesService
    {
        private readonly HttpClient _httpClient;
        readonly IConfiguration _config;
        readonly ITokenAcquisition _tokenAcquisition;

        public TramitesService(HttpClient httpClient, ITokenAcquisition tokenAcquisition, IConfiguration config)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _config = config;
        }

        public async Task<ErrorOr<bool>> ExisteRelacionTipoTramiteSubtipo(int idTipoTramite, int idSubtipoTramite)
        {
            try
            {
                var tokenRegistros = await _tokenAcquisition.GetAccessTokenForAppAsync(
                    _config["Apis:Tramites:Scope"] ?? "",
                    authenticationScheme: JwtBearerDefaults.AuthenticationScheme
                    );

                string strUri = $"odata/SubtiposTramites?$filter=Id eq {idSubtipoTramite} and IdTipoTramite eq {idTipoTramite}";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenRegistros);

                string resp = await _httpClient.GetStringAsync(strUri);
                var result = JsonSerializer.Deserialize<ODataResponse<SubtipoTramiteDto>>(resp);

                List<SubtipoTramiteDto>? resultList = result?.Value;

                if (resultList!.Count > 0)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return ErroresExcepcionMorosidad.FalloServicioTramites;
            }
        }
    }
}
