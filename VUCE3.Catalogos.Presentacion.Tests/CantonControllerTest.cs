using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Cantones.Queries.ObtenerCantones;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.Cantones.Queries.ObtenerCantonPorId;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.CrearCanton;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.EditarCanton;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.EliminarCanton;
using VUCE3.Catalogos.Dominio.Errores;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.EliminarCantones;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.ImportarDatos;


namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class CantonControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerCanton_OK()
        {
            //Arange
            var lstCanton = new List<Canton>()
            {
                new Canton()
                {
                    Id =1,
                    Nombre = "Canton 1",

                },
                new Canton()
                {
                    Id =2,
                    Nombre = "Canton 2",

                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCantonesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstCanton);

            var controller = new CantonesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<CantonDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<CantonDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Canton 1", querydto[0].Nombre);

        }
        [Fact]
        public async Task ObtenerCantonPorId_Ok()
        {
            //Arrange
            var provincia = new Canton()
            {
                Id = 1,
                Nombre = "Canton 1",

            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCantonPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(provincia);

            var controller = new CantonesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<CantonDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CantonDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Canton 1", querydto.Nombre);
        }
        [Fact]
        public async Task ObtenerCantonPorId_Error()
        {
            var error = ErroresCanton.NoEncontrado;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCantonPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new CantonesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerCanton_Error()
        {
            var error = ErroresCanton.NoEncontrado;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCantonesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new CantonesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerCantonPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();            
            var controller = new CantonesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostCanton_DevuelveCreated()
        {
            var provinciaDto = new CantonDto()
            {
                Id = 1,
                Nombre = "Canton 1"

            };

            var provincia = new Canton()
            {
                Id = 1,
                Nombre = "Canton 1"
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCantonCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(provincia);

            var controller = new CantonesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(provinciaDto);

            var okResult = Assert.IsType<CreatedODataResult<CantonDto>>(result);
            var resultCanton = Assert.IsType<CantonDto>(okResult.Value);
            Assert.Equal("Canton 1", resultCanton.Nombre);
        }
        [Fact]
        public async Task PostCanton_DevuelveBadRequest()
        {
            var provinciaDto = new CantonDto()
            {
                Id = 1,
                Nombre = "Canton 1"

            };

            var provincia = new Canton()
            {
                Id = 1,
                Nombre = "Canton 1"
            };            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCantonCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(provincia);            

            var controller = new CantonesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(provinciaDto);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PostCanton_DevuelveProblem()
        {
            var provinciaDto = new CantonDto()
            {
                Id = 1,
                Nombre = "Canton 1"

            };

            var error = Error.Failure();            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCantonCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);
            

            var controller = new CantonesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(provinciaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
        [Fact]
        public async Task PatchCanton_InvalidModel()
        {

            var mockSender = new Mock<ISender>();            

            var controller = new CantonesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<CantonDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchCanton_Ok()
        {
            var provincia = new Canton()
            {
                Id = 1,
                Nombre = "Canton 1"
            };

            var provinciaDto = new CantonDto()
            {
                Id = 1,
                Nombre = "Canton 1"
            };

            var delta = new Delta<CantonDto>(provinciaDto.GetType());
            delta.TrySetPropertyValue(nameof(provinciaDto.Nombre), "Canton 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarCantonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(provincia, provincia));

            var controller = new CantonesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchCanton_NotFound()
        {            
            var error = ErroresCanton.NoEncontrado;

            var provinciaDto = new CantonDto()
            {
                Id = 1,
                Nombre = "Canton 1"
            };

            var delta = new Delta<CantonDto>(provinciaDto.GetType());
            delta.TrySetPropertyValue(nameof(provinciaDto.Nombre), "Canton 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarCantonCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new CantonesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task DeleteCanton_NotFound()
        {
            var error = ErroresCanton.NoEncontrado;         
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarCantonCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);         

            var controller = new CantonesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteCanton_Ok()
        {
            var provincia = new Canton()
            {
                Id = 1,                
                Nombre = "Canton 1"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarCantonCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(provincia);

            var controller = new CantonesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCanton_InvalidModel()
        {           

            var mockSender = new Mock<ISender>();            
            
            var controller = new CantonesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarCantones_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCantonesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new CantonesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarCantones_DevuelveError()
        {

            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCantonesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new CantonesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarCantones_DevuelveErrorModeloInvalido()
        {

            var controller = new CantonesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            IEnumerable<ImportarCantonDto> objetos = new List<ImportarCantonDto>
            {
                new ImportarCantonDto
                {
                    Provincia = 1,
                    CodigoCanton = "01",
                    Canton = "Nombre1"
                }
            };

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", objetos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CantonesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
                new CantonDto { Id = 1, Codigo = "01", Nombre = "Nombre 1", IdProvincia=1 },
                new CantonDto { Id = 2, Codigo = "02", Nombre = "Nombre 2" }
            };

            IEnumerable<ImportarCantonDto> cantones = objetos.Cast<ImportarCantonDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", cantones);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CantonesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarCantonDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new CantonesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new CantonesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarCantonDto {CodigoCanton="01", Canton = "", Provincia=1 }
            };

            IEnumerable<ImportarCantonDto> cantones = objetos.Cast<ImportarCantonDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", cantones);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CantonesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }
    }
}
