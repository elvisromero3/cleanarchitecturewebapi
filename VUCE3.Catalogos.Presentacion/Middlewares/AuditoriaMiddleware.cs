using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Web;
using VUCE3.Catalogos.Presentacion.Servicios.DTO;

namespace VUCE3.Catalogos.Presentacion.Middlewares
{
    [ExcludeFromCodeCoverage]
    public class AuditoriaMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> dicNombreControladorAccion = new Dictionary<string, string>()
        {
            { "Aduanas","aduanas"},
            { "Casa","casas"},
            { "Catalogos","catálogos"},
            { "Clientes","clientes"},
            { "Cultivo","cultivos"},
            { "Empresas","empresas"},
            { "Imagenes","imágenes login"},
            { "NoticiasVuce","noticias home"},
            { "Paises","países"},
            { "Regentes","regentes"},
            { "Variedad","variedades"},
            { "Familia","familias"},
            { "PaisBloqueComercial","pais bloque comercial"},
            { "Productos","productos agrícolas"},
            { "TipoProductos","tipo de productos"},
            { "ExcepcionMorosidad","excepciones morosidad o bloqueos"},
            { "Cantones","cantones"},
            { "Provincias","provincias"},
            { "Distritos","distritos"},
            { "Barrios","barrios"},
            { "BloquesComerciales","bloques comerciales"},
            { "Requisitos","requisitos"},
            { "Sectores","sectores de PROCOMER"},
            { "Sustancia","ANAQ - Sustancias controladas"},
            { "SustanciasControladas","DIGECA - Sustancias Controladas"},
            { "Caracteristicas","características de productos"},
            { "Categoria","categorías de productos"},
            { "Establecimientos","establecimientos"}
        };

        public AuditoriaMiddleware(RequestDelegate next, HttpClient httpClient, IConfiguration configuration)
        {
            _next = next;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, ITokenAcquisition tokenAcquisition)
        {
            var originalBody = context.Response.Body;

            try
            {
                //se requiere crear un memoryStream antes del next para poder obtener el response de vuelta
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    await _next(context);

                    var request = context.Request;

                    //se obtienen los datos del Request requeridos para el registro de Auditoria
                    var pantalla = (request.Headers.TryGetValue("Pantalla", out StringValues pantallaValue))
                        ? HttpUtility.UrlDecode(pantallaValue.ToString()) : null;

                    int? idUsuario = (request.Headers.TryGetValue("IdUsuario", out StringValues idUsuarioValue))
                        ? Convert.ToInt32(idUsuarioValue) : null;

                    var rolUsuario = (request.Headers.TryGetValue("RolUsuario", out StringValues rolUsuarioValue))
                        ? rolUsuarioValue.ToString() : null;

                    var idEntidad = (request.Headers.TryGetValue("IdEntidad", out StringValues idEntidadValue))
                        ? idEntidadValue.ToString() : null;

                    var tipoEntidad = (request.Headers.TryGetValue("TipoEntidad", out StringValues tipoEntidadValue))
                        ? tipoEntidadValue.ToString() : null;

                    var controlador = (request.RouteValues.TryGetValue("controller", out object? controladorValue))
                        ? controladorValue?.ToString() : null;

                    //se valida si el Response generó un código Success
                    //y si existen los datos requeridos para el registro de Auditoria
                    if (context.Request.RouteValues.Count == 0
                        || !context.Response.IsSuccessStatusCode()
                        || pantalla is null
                        || rolUsuario is null
                        || idUsuario is null
                        || controlador is null
                        || idEntidad is null
                        || tipoEntidad is null)
                    {
                        memStream.Position = 0;
                        await memStream.CopyToAsync(originalBody);

                        return;
                    }

                    //se valida y obtiene el nombre del controlador para la acción de Auditoria
                    var controladorAccion = (dicNombreControladorAccion.TryGetValue(controlador, out string? controladorAccionValue))
                        ? controladorAccionValue : string.Empty;

                    //se valida y obtiene el identificador de entidad para la acción de Auditoria
                    var identificadorRegistro = (request.Headers.TryGetValue("IdentificadorRegistro", out StringValues identificadorRegistroValue))
                        ? " con " + HttpUtility.UrlDecode(identificadorRegistroValue.ToString()) : string.Empty;

                    //se construye la acción de la Auditoria
                    string accion;
                    switch (request.Method)
                    {
                        case "GET":
                            accion = "Se han consultado registros de " + controladorAccion + ".";
                            break;

                        case "POST":
                            accion = "Se ha creado un registro de " + controladorAccion + identificadorRegistro + ".";
                            break;

                        case "PATCH":
                            accion = "Se ha modificado el registro de " + controladorAccion + identificadorRegistro + ".";
                            break;

                        case "DELETE":
                            accion = "Se ha eliminado el registro de " + controladorAccion + identificadorRegistro + ".";
                            break;

                        default:
                            accion = "";
                            break;
                    }

                    string? datosAntes = null;
                    string? datosDespues = null;
                    int? idTabla = null;

                    if (request.Method == HttpMethods.Post)
                    {
                        //se obtiene datosDespues
                        memStream.Position = 0;
                        datosDespues = await new StreamReader(memStream).ReadToEndAsync();
                        datosAntes = "{}";
                        if (datosDespues != null)
                        {
                            var datosPost = JsonNode.Parse(datosDespues);
                            if (datosPost?["Id"] != null)
                            {
                                idTabla = datosPost["Id"]!.GetValue<int>();
                            }
                        }
                    }
                    else if (request.Method == HttpMethods.Patch)
                    {
                        //se obtiene datosAntes y datosDespues
                        memStream.Position = 0;
                        var datosJson = await new StreamReader(memStream).ReadToEndAsync();

                        JsonNode? jsonResp = JsonNode.Parse(datosJson);
                        datosAntes = jsonResp?["item1"]?.AsObject().ToJsonString();
                        datosDespues = jsonResp?["item2"]?.AsObject().ToJsonString();
                        if (datosDespues != null)
                        {
                            var datosPatch = JsonNode.Parse(datosDespues);
                            if (datosPatch?["id"] != null)
                            {
                                idTabla = datosPatch["id"]!.GetValue<int>();
                            }
                        }
                    }
                    else if (request.Method == HttpMethods.Delete)
                    {
                        //se obtiene datosAntes
                        memStream.Position = 0;
                        datosAntes = await new StreamReader(memStream).ReadToEndAsync();
                        datosDespues = "{}";
                        if (datosAntes != null)
                        {
                            var datosDelete = JsonNode.Parse(datosAntes);
                            if (datosDelete?["Id"] != null)
                            {
                                idTabla = datosDelete["Id"]!.GetValue<int>();
                            }
                        }
                    }
                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);

                    //se crea el objeto de Auditoria
                    var body = JsonConvert.SerializeObject(new GuardarAuditoriaDto()
                    {
                        Pantalla = pantalla,
                        Accion = accion,
                        IdUsuario = idUsuario.Value,
                        RolUsuario = rolUsuario,
                        IdEntidad = Convert.ToInt32(idEntidad),
                        TipoEntidad = Convert.ToInt32(tipoEntidad),
                        IdentificadorRegistroTabla = idTabla,
                        DatosAntes = datosAntes,
                        DatosDespues = datosDespues
                    });

                    //se obtiene token de auditoria			
                    var tokenAuditoria = await tokenAcquisition.GetAccessTokenForAppAsync(
                        _configuration["Apis:AuditoriaTrazabilidad:Scope"] ?? "",
                        authenticationScheme: JwtBearerDefaults.AuthenticationScheme);

                    //se llama al microservicio de Auditoria para realizar el registro
                    string strUri = _configuration["Apis:AuditoriaTrazabilidad:BaseUrl"] + "/odata/auditoria";
                    StringContent httpContent = new StringContent(body, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenAuditoria);

                    await _httpClient.PostAsync(strUri, httpContent);
                }
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }
}