using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.TiposAccionesExcepcionesMorosidad.Queries.ObtenerTiposAccionesExcepcionesMorosidad;
using VUCE3.Catalogos.Aplicacion.TiposAccionesExcepcionesMorosidad.Queries.ObtenerTiposAccionesExcepcionesMorosidadPorId;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class TiposAccionesExcepcionesMorosidadControllerTest
    {
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerTiposAccionesExcepcionesMorosidad_OK()
        {
            //Arange
            var lstTiposAcciones = new List<TipoAccionExcepcionMorosidad>()
            {
                new TipoAccionExcepcionMorosidad()
                {
                    Id =1,
                    Nombre = "Tipo 1"
                },
                new TipoAccionExcepcionMorosidad()
                {
                    Id =2,
                    Nombre = "Tipo 2"
                },
                new TipoAccionExcepcionMorosidad()
                {
                    Id =3,
                    Nombre = "Tipo 3",
                }
            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerTiposAccionesExcepcionesMorosidadQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstTiposAcciones);

            var controller = new TiposAccionesExcepcionesMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<TipoAccionExcepcionMorosidadDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<TipoAccionExcepcionMorosidadDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Tipo 1", querydto[0].Nombre);
        }

        [Fact]
        public async Task ObtenerTiposAccionesExcepcionesMorosidad_Error()
        {
            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerTiposAccionesExcepcionesMorosidadQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new TiposAccionesExcepcionesMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("500", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerTipoAccionExcepcionMorosidadPorId_Ok()
        {
            //Arrange
            var tipoAccion = new TipoAccionExcepcionMorosidad()
            {
                Id = 1,
                Nombre = "Tipo 1"
            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerTiposAccionesExcepcionesMorosidadPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(tipoAccion);

            var controller = new TiposAccionesExcepcionesMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<TipoAccionExcepcionMorosidadDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<TipoAccionExcepcionMorosidadDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Tipo 1", querydto.Nombre);
        }

        [Fact]
        public async Task ObtenerTipoAccionExcepcionMorosidadPorId_Error()
        {
            var error = ErroresTiposAccionesExcepcionesMorosidad.TipoAccionExcepcionMorosidadNoEncontrado;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerTiposAccionesExcepcionesMorosidadPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new TiposAccionesExcepcionesMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerTipoAccionExcepcionMorosidadPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new TiposAccionesExcepcionesMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }
    }
}
