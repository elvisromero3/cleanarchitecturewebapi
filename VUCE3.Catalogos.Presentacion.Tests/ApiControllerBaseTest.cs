using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class ApiControllerBaseTest : ApiControllerBase
    {

        private static readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();
        public ApiControllerBaseTest() : base(mockLocalizer.Object){}

        [Fact]
        public void Problem_DevuelveCodigoCorrecto()
        {
            var result = Problem(new List<Error> { });
            var objectResult = Assert.IsType<ObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal(500, problemDetails.Status);

            result = Problem(new List<Error> { Error.Conflict() });
            objectResult = Assert.IsType<ObjectResult>(result);
            problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal(409, problemDetails.Status);

            result = Problem(new List<Error> { Error.Validation() });
            objectResult = Assert.IsType<ObjectResult>(result);
            problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal(400, problemDetails.Status);

            result = Problem(new List<Error> { Error.NotFound() });
            objectResult = Assert.IsType<ObjectResult>(result);
            problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal(404, problemDetails.Status);

            result = Problem(new List<Error> { Error.Forbidden() });
            objectResult = Assert.IsType<ObjectResult>(result);
            problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal(403, problemDetails.Status);

            result = Problem(new List<Error> { Error.Unauthorized() });
            objectResult = Assert.IsType<ObjectResult>(result);
            problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal(401, problemDetails.Status);

            result = Problem(new List<Error> { Error.Failure() });
            objectResult = Assert.IsType<ObjectResult>(result);
            problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal(500, problemDetails.Status);

        }

        [Fact]
        public void Problem_IncludesTraceId_WhenInsertarTrazaIsTrue()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "TestTraceId12345";
            ControllerContext = new ControllerContext { HttpContext = httpContext };

            var errors = new List<Error> { Error.Failure("TestError") };

            // Act
            var result = Problem(errors, insertarTraza: true);
            var objectResult = Assert.IsType<ObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);

            // Assert
            var errorsExtension = Assert.IsType<Dictionary<string, List<string>>>(problemDetails.Extensions["errors"]);
            Assert.True(errorsExtension.ContainsKey("TestError"));
            Assert.Contains("TestTraceId12345", errorsExtension["TestError"][0]);
        }

    }
}
