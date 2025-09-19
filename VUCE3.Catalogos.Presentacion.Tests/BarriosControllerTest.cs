using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.CrearBarrio;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.EditarBarrio;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.EliminarBarrio;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.EliminarBarrios;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Barrios.Queries.ObtenerBarrioPorId;
using VUCE3.Catalogos.Aplicacion.Barrios.Queries.ObtenerBarrios;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.EliminarDistritos;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Tests.Helper;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class BarriosControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerBarrios_OK()
        {
            //Arange
            var lstBarrios = new List<Barrio>()
            {
                new Barrio()
                {
                    Id =1,
                    Codigo = "1",
                    Nombre = "Barrio 1",
                    IdDistrito = 1

                },
                new Barrio()
                {
                    Id =2,
                    Codigo = "2",
                    Nombre = "Barrio 2",
                    IdDistrito=2
                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerBarriosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstBarrios);

            var controller = new BarriosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<BarrioDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<BarrioDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Barrio 1", querydto[0].Nombre);
        }

        [Fact]
        public async Task ObtenerBarrios_Error()
        {
            var error = ErroresBarrio.BarrioNoEncontrado;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerBarriosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new BarriosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerBarrioPorId_Ok()
        {
            //Arrange
            var barrio = new Barrio()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Barrio 1",
                IdDistrito = 1
            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerBarrioPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(barrio);

            var controller = new BarriosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<BarrioDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BarrioDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Barrio 1", querydto.Nombre);
        }

        [Fact]
        public async Task ObtenerBarrioPorId_Error()
        {
            var error = ErroresBarrio.BarrioNoEncontrado;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerBarrioPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new BarriosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }
        
        [Fact]
        public async Task ObtenerBarrioPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();
            var controller = new BarriosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostBarrio_DevuelveCreated()
        {
            var barrioDto = new BarrioDto()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Barrio 1",
                IdDistrito = 1,
                IdCanton = 1,
                IdProvincia = 1
            };

            var barrio = new Barrio()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Barrio 1",
                IdDistrito = 1
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearBarrioCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(barrio);

            var controller = new BarriosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(barrioDto);

            var okResult = Assert.IsType<CreatedODataResult<BarrioDto>>(result);
            var resultBarrio = Assert.IsType<BarrioDto>(okResult.Value);
            Assert.Equal("Barrio 1", resultBarrio.Nombre);
        }

        [Fact]
        public async Task PostBarrio_DevuelveBadRequest()
        {
            var barrioDto = new BarrioDto()
            {
                Id = 1,
                Codigo = "123",
                Nombre = "Barrio 1",
                IdDistrito = 1,
                IdCanton = 1,
                IdProvincia = 1
            };

            var barrio = new Barrio()
            {
                Id = 1,
                Codigo = "12",
                Nombre = "Barrio 1",
                IdDistrito = 1
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearBarrioCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(barrio);

            var controller = new BarriosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(barrioDto);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostBarrio_DevuelveProblem()
        {
            var barrioDto = new BarrioDto()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Barrio 1",
                IdDistrito = 1,
                IdCanton = 1,
                IdProvincia = 1
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearBarrioCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new BarriosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(barrioDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task PatchBarrio_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new BarriosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<BarrioDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchBarrio_Ok()
        {
            var barrio = new Barrio()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Barrio 1",
                IdDistrito = 1
            };

            var barrioDto = new BarrioDto()
            {
                Id = 1,
                Codigo = "2",
                Nombre = "Barrio 1",
                IdDistrito = 1,
                IdCanton = 1,
                IdProvincia = 1
            };

            var delta = new Delta<BarrioDto>(barrioDto.GetType());
            delta.TrySetPropertyValue(nameof(barrioDto.Nombre), "Barrio 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarBarrioCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(barrio, barrio));

            var controller = new BarriosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchBarrio_NotFound()
        {
            var error = ErroresBarrio.BarrioNoEncontrado;

            var barrioDto = new BarrioDto()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Barrio 1",
                IdDistrito = 1,
                IdCanton = 1,
                IdProvincia = 1
            };

            var delta = new Delta<BarrioDto>(barrioDto.GetType());
            delta.TrySetPropertyValue(nameof(barrioDto.Nombre), "Barrio 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarBarrioCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new BarriosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task DeleteBarrio_NotFound()
        {
            var error = ErroresBarrio.BarrioNoEncontrado;
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarBarrioCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new BarriosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task DeleteBarrio_Ok()
        {
            var barrio = new Barrio()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Distrito 1",
                IdDistrito = 1
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarBarrioCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(barrio);

            var controller = new BarriosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteBarrio_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new BarriosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarBarrios_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarBarriosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new BarriosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarBarrios_DevuelveError()
        {

            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarBarriosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new BarriosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarBarrios_DevuelveErrorModeloInvalido()
        {

            var controller = new BarriosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            IEnumerable<ImportarBarrioDto> objetos = new List<ImportarBarrioDto>
            {
                new ImportarBarrioDto
                {
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 1,
                    CodigoBarrio = "01",
                    Barrio = "Nombre1"
                }
            };

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", objetos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new BarriosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
                new BarrioDto { Id = 1, Codigo = "01", Nombre = "Nombre 1", IdCanton=1, IdProvincia=1 },
                new BarrioDto { Id = 2, Codigo = "02", Nombre = "Nombre 2" }
            };

            IEnumerable<ImportarBarrioDto> barrios = objetos.Cast<ImportarBarrioDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", barrios);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new BarriosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarBarrioDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new BarriosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new BarriosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarBarrioDto {CodigoBarrio="01", Barrio = "", Distrito = 1, Canton = 1, Provincia=1 }
            };

            IEnumerable<ImportarBarrioDto> barrios = objetos.Cast<ImportarBarrioDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", barrios);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new BarriosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }
    }
}
