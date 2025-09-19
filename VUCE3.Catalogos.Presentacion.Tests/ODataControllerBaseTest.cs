using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class ODataControllerBaseTest : ODataControllerBase
    {
        private static readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();
        public ODataControllerBaseTest() : base(mockLocalizer.Object) { }

        [Fact]
        public void Problem_DevuelveCodigoCorrecto()
        {

            LocalizedString? resourceLang = null;

            var result = Problem(new List<Error> { });
            var objectResult = Assert.IsType<ObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal(500, problemDetails.Status);

            resourceLang = SetLocalizedString("General.Conflict", "A conflict error has occurred.");
            mockLocalizer.Setup(r => r["General.Conflict"]).Returns(resourceLang);
            result = Problem(new List<Error> { Error.Conflict() });            
            var oDataErrorResult = Assert.IsType<ODataErrorResult>(result);
            var oDataError = Assert.IsType<ODataError>(oDataErrorResult.Error);            
            Assert.Equal("409", oDataError.ErrorCode);

            resourceLang = SetLocalizedString("General.Validation", "A validation error has occurred.");
            mockLocalizer.Setup(r => r["General.Validation"]).Returns(resourceLang);
            result = Problem(new List<Error> { Error.Validation() });
            oDataErrorResult = Assert.IsType<ODataErrorResult>(result);
            oDataError = Assert.IsType<ODataError>(oDataErrorResult.Error);
            Assert.Equal("400", oDataError.ErrorCode);

            resourceLang = SetLocalizedString("General.NotFound", "A 'Not Found' error has occurred.");
            mockLocalizer.Setup(r => r["General.NotFound"]).Returns(resourceLang);
            result = Problem(new List<Error> { Error.NotFound() });
            oDataErrorResult = Assert.IsType<ODataErrorResult>(result);
            oDataError = Assert.IsType<ODataError>(oDataErrorResult.Error);
            Assert.Equal("404", oDataError.ErrorCode);

            resourceLang = SetLocalizedString("General.Forbidden", "A 'Forbidden' error has occurred.");
            mockLocalizer.Setup(r => r["General.Forbidden"]).Returns(resourceLang);
            result = Problem(new List<Error> { Error.Forbidden() });
            oDataErrorResult = Assert.IsType<ODataErrorResult>(result);
            oDataError = Assert.IsType<ODataError>(oDataErrorResult.Error);
            Assert.Equal("403", oDataError.ErrorCode);

            resourceLang = SetLocalizedString("General.Unauthorized", "An 'Unauthorized' error has occurred.");
            mockLocalizer.Setup(r => r["General.Unauthorized"]).Returns(resourceLang);
            result = Problem(new List<Error> { Error.Unauthorized() });
            oDataErrorResult = Assert.IsType<ODataErrorResult>(result);
            oDataError = Assert.IsType<ODataError>(oDataErrorResult.Error);
            Assert.Equal("401", oDataError.ErrorCode);

            resourceLang = SetLocalizedString("General.Failure", "A failure has occurred");
            mockLocalizer.Setup(r => r["General.Failure"]).Returns(resourceLang);
            result = Problem(new List<Error> { Error.Failure() });
            oDataErrorResult = Assert.IsType<ODataErrorResult>(result);
            oDataError = Assert.IsType<ODataError>(oDataErrorResult.Error);
            Assert.Equal("500", oDataError.ErrorCode);
        }

        private static LocalizedString SetLocalizedString(string key, string value)
        {
            return new LocalizedString(key, value);
        }

        [Fact]
        public void Problem_InsertaTrazaEnMensajeSiInsertarTrazaEsTrue()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "TestTraceId12345";
            ControllerContext = new ControllerContext { HttpContext = httpContext };

            var errors = new List<Error> { Error.Failure("TestError") };

            // Act
            var result = Problem(errors, insertarTraza: true);
            var odataErrorResult = Assert.IsType<ODataErrorResult>(result);
            var odataError = Assert.IsType<ODataError>(odataErrorResult.Error);

            // Assert
            Assert.Contains("500", odataError.ErrorCode);
            Assert.Contains("TestTraceId12345.", odataError.Message);
        }

    }
}
