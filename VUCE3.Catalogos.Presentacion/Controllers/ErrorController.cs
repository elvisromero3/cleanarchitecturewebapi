using ErrorOr;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    [Route("odata/[controller]")]
    public class ErrorController : ODataControllerBase
    {
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(IStringLocalizer<ILocalization> localizer, ILogger<ErrorController> logger) : base(localizer)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return loggearProblem("GET");
        }

        [HttpPost]
        public IActionResult Post()
        {
            return loggearProblem("POST");
        }

        [HttpPatch]
        public IActionResult Patch()
        {
            return loggearProblem("PATCH");
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            return loggearProblem("DELETE");
        }

        private IActionResult loggearProblem(string verbo)
        {
            _logger.LogError("Metodo: {Verbo}\nTraza de error: {IdTraza}", verbo, HttpContext.TraceIdentifier);
            return Problem([Error.Failure(code: "ErrorController.GeneralError")], insertarTraza: true);
        }
    }
}
