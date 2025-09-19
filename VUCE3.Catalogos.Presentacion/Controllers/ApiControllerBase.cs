using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    [ApiController]
    [Authorize(Roles = "Catalogos")]
    public class ApiControllerBase : ControllerBase
    {
        private readonly IStringLocalizer<ILocalization> _localizer;

        public ApiControllerBase(IStringLocalizer<ILocalization> localizer)
        {
            _localizer = localizer;
        }

        protected IActionResult Problem(List<Error> errors, bool insertarTraza = false)
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

            var problemDetails = (ProblemDetails?)Problem(statusCode: statusCode).Value;

            var dictionaryErrors = new Dictionary<string, List<string>>();
            
            foreach (var errorCode in errors.Select(e =>e.Code))
            {
                if (!dictionaryErrors.TryGetValue(errorCode, out List<string>? value))
                {
                    value = new List<string>();
                    dictionaryErrors.Add(errorCode, value);
                }

                if (insertarTraza)
                {
                    var traceId = HttpContext.TraceIdentifier;
                    value.Add($"{_localizer["MensajeTraza"]} {traceId}.");
                }
                else
                {
                    value.Add(_localizer[errorCode]);
                }

            }

            problemDetails?.Extensions.Add("errors", dictionaryErrors);

            return new ObjectResult(problemDetails);
        }
    }
}
