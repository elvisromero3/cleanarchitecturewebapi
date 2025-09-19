using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.CrearCaracteristica;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EditarCaracteristica;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EliminarCaracteristica;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EliminarCaracteristicas;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Queries.ObtenerCaracteristicaPorId;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Queries.ObtenerCaracteristicas;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Tests.Helper;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class CaracteristicasControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerCaracteristicas_OK()
        {
            //Arange
            var lstCaracteristicas = new List<Caracteristica>()
            {
                new Caracteristica()
                {
                    Id =1,
                    IdInstitucion = 1,
                    Nombre = "Caracteristica 1",

                },
                new Caracteristica()
                {
                    Id =2,
                    IdInstitucion =2,
                    Nombre = "Caracteristica 2"
                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCaracteristicasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstCaracteristicas);

            var controller = new CaracteristicasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<CaracteristicaDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<CaracteristicaDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Caracteristica 1", querydto[0].Nombre);
        }

        [Fact]
        public async Task ObtenerCaracteristicaPorId_Ok()
        {
            //Arrange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 1,
                Nombre = "Caracteristica 1",

            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCaracteristicaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(caracteristica);

            var controller = new CaracteristicasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<CaracteristicaDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CaracteristicaDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Caracteristica 1", querydto.Nombre);
        }

        [Fact]
        public async Task ObtenerCaracteristicaPorId_Error()
        {
            var error = ErroresCaracteristica.CaracteristicaInstitucionNoEncontrada;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCaracteristicaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new CaracteristicasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerCaracteristica_Error()
        {
            var error = ErroresCaracteristica.CaracteristicaNoEncontrada;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCaracteristicasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new CaracteristicasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerCaracteristicaPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();
            var controller = new CaracteristicasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostCaracteristica_DevuelveCreated()
        {
            var caracteristicaDto = new CaracteristicaDto()
            {
                Id = 1,
                IdInstitucion = 1,
                Nombre = "Caracteristica 1"

            };

            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 1,
                Nombre = "Caracteristica 1"
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCaracteristicaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(caracteristica);

            var controller = new CaracteristicasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(caracteristicaDto);

            var okResult = Assert.IsType<CreatedODataResult<CaracteristicaDto>>(result);
            var resultSector = Assert.IsType<CaracteristicaDto>(okResult.Value);
            Assert.Equal("Caracteristica 1", resultSector.Nombre);
        }

        [Fact]
        public async Task PostCaracteristica_DevuelveBadRequest()
        {
            var caracteristicaDto = new CaracteristicaDto()
            {
                Id = 1,
                Nombre = "Caracteristica 1"

            };

            var caracteristica = new Caracteristica()
            {
                Id = 1,
                Nombre = "Caracteristica 1"
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCaracteristicaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(caracteristica);

            var controller = new CaracteristicasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(caracteristicaDto);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostCaracteristica_DevuelveProblem()
        {
            var caracteristicaDto = new CaracteristicaDto()
            {
                Id = 1,
                Nombre = "Caracteristica 1"

            };

            var error = Error.Failure();

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCaracteristicaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);


            var controller = new CaracteristicasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(caracteristicaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task PatchCaracteristica_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new CaracteristicasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<CaracteristicaDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchCaracteristica_Ok()
        {
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                Nombre = "Caracteristica 1"
            };

            var caracteristicaDto = new CaracteristicaDto()
            {
                Id = 1,
                Nombre = "Caracteristica 1"
            };

            var delta = new Delta<CaracteristicaDto>(caracteristicaDto.GetType());
            delta.TrySetPropertyValue(nameof(caracteristicaDto.Nombre), "Caracteristica 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarCaracteristicaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(caracteristica, caracteristica));

            var controller = new CaracteristicasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchCaracteristica_NotFound()
        {
            var error = ErroresCaracteristica.CaracteristicaNoEncontrada;

            var caracteristicaDto = new CaracteristicaDto()
            {
                Id = 1,
                Nombre = "Caracteristica 1"
            };

            var delta = new Delta<CaracteristicaDto>(caracteristicaDto.GetType());
            delta.TrySetPropertyValue(nameof(caracteristicaDto.Nombre), "Caracteristica 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarCaracteristicaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new CaracteristicasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task DeleteCaracteristica_NotFound()
        {
            var error = ErroresCaracteristica.CaracteristicaNoEncontrada;
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarCaracteristicaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new CaracteristicasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task DeleteCaracteristica_Ok()
        {
            var grupo = new Caracteristica()
            {
                Id = 1,
                Nombre = "Caracteristica 1"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarCaracteristicaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(grupo);

            var controller = new CaracteristicasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCaracteristica_InvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new CaracteristicasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task BorradoMasivo_Ok()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCaracteristicasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);
            var controller = new CaracteristicasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task BorradoMasivo_Error()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });
            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCaracteristicasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());
            var controller = new CaracteristicasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            
            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task BorradoMasivo_InvalidModel()
        {
            var mockSender = new Mock<ISender>();
            var controller = new CaracteristicasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");
            var result = await controller.BorradoMasivo(new ODataActionParameters());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_Ok()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", new List<ImportarCaracteristicaDto> { new ImportarCaracteristicaDto { Nombre = "Caracteristica 1", Institucion = "PROCOMER", IdInstitucion = 2 } });
            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);
            var controller = new CaracteristicasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            
            //Act
            var result = await controller.ImportarDatos(param);
            
            //Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_Error()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", new List<ImportarCaracteristicaDto> { new ImportarCaracteristicaDto { Nombre = "Caracteristica 1", Institucion = "PROCOMER" } });

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());
            
            var controller = new CaracteristicasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ImportarDatos_InvalidModel()
        {
            //Arrange
            var mockSender = new Mock<ISender>();
            var controller = new CaracteristicasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            //Act
            var result = await controller.ImportarDatos(new ODataActionParameters());

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }
    }
}
