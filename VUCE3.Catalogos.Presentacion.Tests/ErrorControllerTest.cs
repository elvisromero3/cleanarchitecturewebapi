using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class ErrorControllerTest
    {
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();
        private readonly Mock<ILogger<ErrorController>> mockLogger = new Mock<ILogger<ErrorController>>();

        [Fact]
        public void Get_HandleError_ReturnsProblemDetails()
        {
            // Arrange
            var exceptionHandlerFeatureMock = new Mock<IExceptionHandlerFeature>();
            exceptionHandlerFeatureMock.Setup(e => e.Error).Returns(new Exception("Test exception"));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Features.Get<IExceptionHandlerFeature>()).Returns(exceptionHandlerFeatureMock.Object);
            httpContextMock.Setup(c => c.Request.Path).Returns("/test-path");

            var controller = new ErrorController(mockLocalizer.Object, mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };

            // Act
            var result = controller.Get();

            // Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("500", errorRes.Error.ErrorCode);
        }

        [Fact]
        public void Post_HandleError_ReturnsProblemDetails()
        {
            // Arrange
            var exceptionHandlerFeatureMock = new Mock<IExceptionHandlerFeature>();
            exceptionHandlerFeatureMock.Setup(e => e.Error).Returns(new Exception("Test exception"));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Features.Get<IExceptionHandlerFeature>()).Returns(exceptionHandlerFeatureMock.Object);
            httpContextMock.Setup(c => c.Request.Path).Returns("/test-path");

            var controller = new ErrorController(mockLocalizer.Object, mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };

            // Act
            var result = controller.Post();

            // Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("500", errorRes.Error.ErrorCode);
        }

        [Fact]
        public void Patch_HandleError_ReturnsProblemDetails()
        {
            // Arrange
            var exceptionHandlerFeatureMock = new Mock<IExceptionHandlerFeature>();
            exceptionHandlerFeatureMock.Setup(e => e.Error).Returns(new Exception("Test exception"));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Features.Get<IExceptionHandlerFeature>()).Returns(exceptionHandlerFeatureMock.Object);
            httpContextMock.Setup(c => c.Request.Path).Returns("/test-path");

            var controller = new ErrorController(mockLocalizer.Object, mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };

            // Act
            var result = controller.Patch();

            // Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("500", errorRes.Error.ErrorCode);
        }

        [Fact]
        public void Delete_HandleError_ReturnsProblemDetails()
        {
            // Arrange
            var exceptionHandlerFeatureMock = new Mock<IExceptionHandlerFeature>();
            exceptionHandlerFeatureMock.Setup(e => e.Error).Returns(new Exception("Test exception"));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Features.Get<IExceptionHandlerFeature>()).Returns(exceptionHandlerFeatureMock.Object);
            httpContextMock.Setup(c => c.Request.Path).Returns("/test-path");

            var controller = new ErrorController(mockLocalizer.Object, mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };

            // Act
            var result = controller.Delete();

            // Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("500", errorRes.Error.ErrorCode);
        }
    }
}
