using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Moq;
using VUCE3.Catalogos.Aplicacion.EstadosExcepcionesMorosidad.Queries.ObtenerEstadosExcepcionesMorosidad;
using VUCE3.Catalogos.Aplicacion.EstadosExcepcionesMorosidad.Queries.ObtenerEstadosExcepcionesMorosidadPorId;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class EstadosExcepcionesMorosidadControllerTest
    {
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerEstadosExcepcionesMorosidad_OK()
        {
            //Arange
            var lstEstados = new List<EstadoExcepcionMorosidad>()
            {
                new EstadoExcepcionMorosidad()
                {
                    Id =1,
                    Nombre = "Estado 1"
                },
                new EstadoExcepcionMorosidad()
                {
                    Id =2,
                    Nombre = "Estado 2"
                },
                new EstadoExcepcionMorosidad()
                {
                    Id =3,
                    Nombre = "Estado 3",
                }
            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerEstadosExcepcionesMorosidadQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstEstados);

            var controller = new EstadosExcepcionesMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<EstadoExcepcionMorosidadDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<EstadoExcepcionMorosidadDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Estado 1", querydto[0].Nombre);
        }

        [Fact]
        public async Task ObtenerEstadosExcepcionesMorosidad_Error()
        {
            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerEstadosExcepcionesMorosidadQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new EstadosExcepcionesMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("500", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerEstadoExcepcionMorosidadPorId_Ok()
        {
            //Arrange
            var estado = new EstadoExcepcionMorosidad()
            {
                Id = 1,
                Nombre = "Tipo 1"
            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerEstadosExcepcionesMorosidadPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(estado);

            var controller = new EstadosExcepcionesMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<EstadoExcepcionMorosidadDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<EstadoExcepcionMorosidadDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Tipo 1", querydto.Nombre);
        }

        [Fact]
        public async Task ObtenerEstadoExcepcionMorosidadPorId_Error()
        {
            var error = ErroresEstadosExcepcionesMorosidad.EstadoExcepcionMorosidadNoEncontrado;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerEstadosExcepcionesMorosidadPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new EstadosExcepcionesMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerEstadoExcepcionMorosidadPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new EstadosExcepcionesMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }
    }
}
