using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using VUCE3.Catalogos.Presentacion.Resources;


namespace VUCE3.Catalogos.Presentacion.Controllers
{
    [Authorize(Roles = "Catalogos")]
    public class ODataControllerBase : ODataController
    {

        private readonly IStringLocalizer<ILocalization> _localizer;

        public ODataControllerBase(IStringLocalizer<ILocalization> localizer)
        {
            _localizer = localizer;
        }
        protected IActionResult Problem(List<Error> errors, bool traducirMensaje = true, bool insertarTraza = false)
        {
            if (errors.Count is 0)
            {
                return Problem();
            }

            var statusCode = errors[0].Type switch
            {
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError,
            };

            var problemDetails = new List<ODataErrorDetail>();

            foreach (var error in errors)
            {
                var mensaje = error.Description;
                if (traducirMensaje)
                {
                    mensaje = _localizer[error.Code];
                }
                problemDetails.Add(new ODataErrorDetail() { ErrorCode = error.Code, Message = mensaje, Target = "" });
            }

            var odataError = new ODataError()
            {
                ErrorCode = statusCode.ToString(),
                Message = _localizer[errors[0].Code],
                Details = problemDetails
            };

            // Obtener y registrar traceId
            if (insertarTraza)
            {
                odataError.Message = $"{_localizer["MensajeTraza"]} {HttpContext.TraceIdentifier}.";
            }

            return ODataErrorResult(odataError);
        }
        protected IActionResult InvalidModel(ModelStateDictionary modelErrores, string controlador)
        {

            var problemDetails = new List<ODataErrorDetail>();

            foreach (var error in modelErrores)
            {

                problemDetails.Add(new ODataErrorDetail() { ErrorCode = StatusCodes.Status400BadRequest.ToString(), Target = error.Key, Message = error.Value.Errors[0].Exception == null ? error.Value.Errors[0].ErrorMessage : error.Value.Errors[0].Exception?.Message });
            }

            var odataError = new ODataError()
            {
                ErrorCode = "400",
                Target = controlador + ".ModeloInvalido",
                Message = _localizer["ValidacionRequeridosNulos"],
                Details = problemDetails
            };

            return ODataErrorResult(odataError);
        }

        protected IActionResult InvalidModel(List<ODataErrorDetail> modelErrores, string controlador)
        {

            var odataError = new ODataError()
            {
                ErrorCode = StatusCodes.Status400BadRequest.ToString(),
                Target = controlador + ".ModeloInvalido",
                Message = _localizer["ValidacionRequeridosNulos"],
                Details = modelErrores
            };

            return ODataErrorResult(odataError);
        }

        protected IActionResult EstructuraArchivoImportacionInvalido()
        {
            var odataError = new ODataError()
            {
                ErrorCode = StatusCodes.Status400BadRequest.ToString(),
                Target = "EstructuraFicheroInvalida",
                Message = _localizer["ValidacionFicheroImportar"],
            };

            return ODataErrorResult(odataError);
        }
    }
}
