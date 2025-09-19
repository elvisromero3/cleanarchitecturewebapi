using ErrorOr;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json;
using System.Text.Json.Serialization;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Aplicacion.Servicios.DTO;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Servicios
{
   
    public class AccesoGestionUsuariosService : IAccesoGestionUsuariosService
    {
        private readonly HttpClient _httpClient;
        readonly IConfiguration _config;
        readonly ITokenAcquisition _tokenAcquisition;
        public AccesoGestionUsuariosService(HttpClient httpClient, ITokenAcquisition tokenAcquisition, IConfiguration config)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _config = config;
        }

        public async Task<ErrorOr<bool>> ExisteRelacionAduanaInstitucionesAutorizadas(int idAduana)
        {
            try
            {
                var tokenRegistros = await _tokenAcquisition.GetAccessTokenForAppAsync(
             _config["Apis:AccesoGestionUsuarios:Scope"] ?? "",
             authenticationScheme: JwtBearerDefaults.AuthenticationScheme
             );

                string strUri = $"odata/AduanasInstitucionesUsuarios?$filter=IdAduana eq {idAduana}";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenRegistros);

                string resp = await _httpClient.GetStringAsync(strUri);
                var result = JsonSerializer.Deserialize<ODataResponse<AduanaInstitucionUsuarioDto>>(resp);

                List<AduanaInstitucionUsuarioDto>? resultList = result?.Value;
               

                if (resultList!.Count > 0)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return ErroresProfesionales.FalloServicioAccesoGestionUsuario;
            }
        }

        public async Task<ErrorOr<List<InstitucionDto>>> ObtenerInstituciones()
        {
            try
            {
                var tokenRegistros = await _tokenAcquisition.GetAccessTokenForAppAsync(
             _config["Apis:AccesoGestionUsuarios:Scope"] ?? "",
             authenticationScheme: JwtBearerDefaults.AuthenticationScheme
             );

                string strUri = $"odata/Instituciones";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenRegistros);

                string resp = await _httpClient.GetStringAsync(strUri);
                var result = JsonSerializer.Deserialize<ODataResponse<InstitucionDto>>(resp);

                List<InstitucionDto>? resultList = result?.Value;

                return resultList!;
            }
            catch
            {
                return  ErroresProfesionales.FalloServicioAccesoGestionUsuario;
            }
        }
    }
}
