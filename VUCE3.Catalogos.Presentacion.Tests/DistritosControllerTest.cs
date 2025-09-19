using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.CrearDistrito;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.EditarDistrito;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.EliminarDistrito;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.EliminarDistritos;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Distritos.Queries.ObtenerDistritoPorId;
using VUCE3.Catalogos.Aplicacion.Distritos.Queries.ObtenerDistritos;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Tests.Helper;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class DistritosControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerDistritos_OK()
        {
            //Arange
            var lstDistrito = new List<Distrito>()
            {
                new Distrito()
                {
                    Id =1,
                    Codigo = "1",
                    Nombre = "Distrito 1",
                    IdCanton = 1

                },
                new Distrito()
                {
                    Id =2,
                    Codigo = "2",
                    Nombre = "Distrito 2",
                    IdCanton=2
                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerDistritosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstDistrito);

            var controller = new DistritosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<DistritoDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<DistritoDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Distrito 1", querydto[0].Nombre);

        }
        [Fact]
        public async Task ObtenerDistritoPorId_Ok()
        {
            //Arrange
            var distrito = new Distrito()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Distrito 1",
                IdCanton = 1
            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerDistritoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(distrito);

            var controller = new DistritosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<DistritoDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<DistritoDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Distrito 1", querydto.Nombre);
        }
        [Fact]
        public async Task ObtenerDistritoPorId_Error()
        {
            var error = ErroresDistrito.DistritoNoEncontrado;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerDistritoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new DistritosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }
        [Fact]
        public async Task ObtenerDistrito_Error()
        {
            var error = ErroresDistrito.DistritoNoEncontrado;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerDistritosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new DistritosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }
        [Fact]
        public async Task ObtenerDistritoPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();
            var controller = new DistritosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostDistrito_DevuelveCreated()
        {
            var distritoDto = new DistritoDto()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Distrito 1",
                IdCanton = 1,
                IdProvincia = 1
            };

            var distrito = new Distrito()
            {
                Id = 1,
                Codigo= "1",    
                Nombre = "Distrito 1",
                IdCanton = 1
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearDistritoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(distrito);

            var controller = new DistritosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(distritoDto);

            var okResult = Assert.IsType<CreatedODataResult<DistritoDto>>(result);
            var resultDistrito = Assert.IsType<DistritoDto>(okResult.Value);
            Assert.Equal("Distrito 1", resultDistrito.Nombre);
        }
        [Fact]
        public async Task PostDistrito_DevuelveBadRequest()
        {
            var distritoDto = new DistritoDto()
            {
                Id = 1,
                Codigo = "123",
                Nombre = "Distrito 1",
                IdCanton = 1,
                IdProvincia = 1
            };

            var distrito = new Distrito()
            {
                Id = 1,
                Codigo = "12",
                Nombre = "Distrito 1",
                IdCanton = 1
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearDistritoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(distrito);

            var controller = new DistritosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(distritoDto);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PostDistrito_DevuelveProblem()
        {
            var distritoDto = new DistritoDto()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Distrito 1",
                IdCanton = 1,
                IdProvincia = 1
            };
            
            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearDistritoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new DistritosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(distritoDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
        [Fact]
        public async Task PatchDistrito_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new DistritosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<DistritoDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchDistrito_Ok()
        {
            var distrito = new Distrito()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Distrito 1",
                IdCanton = 1
            };

            var distritoDto = new DistritoDto()
            {
                Id = 1,
                Codigo = "2",
                Nombre = "Distrito 1",
                IdCanton = 1,
                IdProvincia = 1
            };

            var delta = new Delta<DistritoDto>(distritoDto.GetType());
            delta.TrySetPropertyValue(nameof(distritoDto.Nombre), "Distrito 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarDistritoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(distrito, distrito));

            var controller = new DistritosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchDistrito_NotFound()
        {
            var error = ErroresDistrito.DistritoNoEncontrado;

            var distritoDto = new DistritoDto()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Distrito 1",
                IdCanton = 1,
                IdProvincia = 1
            };

            var delta = new Delta<DistritoDto>(distritoDto.GetType());
            delta.TrySetPropertyValue(nameof(distritoDto.Nombre), "Distrito 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarDistritoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new DistritosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }

        [Fact]
        public async Task DeleteDistrito_NotFound()
        {
            var error = ErroresDistrito.DistritoNoEncontrado;
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarDistritoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new DistritosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task DeleteDistrito_Ok()
        {
            var distrito = new Distrito()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Distrito 1",
                IdCanton = 1
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarDistritoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(distrito);

            var controller = new DistritosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteDistrito_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new DistritosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarDistritos_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarDistritosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new DistritosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarDistritos_DevuelveError()
        {

            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarDistritosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new DistritosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarDistritos_DevuelveErrorModeloInvalido()
        {

            var controller = new DistritosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            IEnumerable<ImportarDistritoDto> objetos = new List<ImportarDistritoDto>
            {
                new ImportarDistritoDto
                {
                    Provincia = 1,
                    Canton = 1,
                    CodigoDistrito = "01",
                    Distrito = "Nombre1"
                }
            };

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", objetos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new DistritosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_FicheroEstructuraError()
        {
            //Arrange
            IEnumerable<object> objetos = new List<object>
            {
                new DistritoDto { Id = 1, Codigo = "01", Nombre = "Nombre 1", IdCanton=1, IdProvincia=1 },
                new DistritoDto { Id = 2, Codigo = "02", Nombre = "Nombre 2" }
            };

            IEnumerable<ImportarDistritoDto> distritos = objetos.Cast<ImportarDistritoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", distritos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new DistritosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveError()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", new List<ImportarDistritoDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new DistritosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveErrorModeloInvalido()
        {
            //Arrange
            var controller = new DistritosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("Modo", "El campo Modo es requerido");

            //Act
            var result = await controller.ImportarDatos(new ODataActionParameters());

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_FicheroDatosNullError()
        {
            //Arrange
            IEnumerable<object> objetos = new List<object>
            {
                new ImportarDistritoDto {CodigoDistrito="01", Distrito = "", Canton = 1, Provincia=1 }
            };

            IEnumerable<ImportarDistritoDto> distritos = objetos.Cast<ImportarDistritoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", distritos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new DistritosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }
    }
}
