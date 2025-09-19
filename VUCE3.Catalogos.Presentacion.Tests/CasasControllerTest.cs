using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Casa.Queries.ObtenerCasas;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.Casa.Queries.ObtenerCasaPorId;
using VUCE3.Catalogos.Aplicacion.Casa.Commands.CrearCasa;
using VUCE3.Catalogos.Aplicacion.Casa.Commands.EditarCasa;
using VUCE3.Catalogos.Aplicacion.Casa.Commands.EliminarCasa;
using VUCE3.Catalogos.Dominio.Errores;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Aplicacion.Casa.Commands.EliminarCasas;
using VUCE3.Catalogos.Aplicacion.Casa.Commands.ImportarDatos;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class CasasControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerCasas_OK()
        {
            //Arange
            var lstCasas = new List<Casa>()
            {
                new Casa()
                {
                    Id =1,
                    Codigo = "1",
                    Nombre = "CASA 1",

                },
                new Casa()
                {
                    Id =2,
                    Codigo = "1",
                    Nombre = "CASA 2",

                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCasasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstCasas);

            var controller = new CasaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<CasaDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<CasaDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("CASA 1", querydto[0].Nombre);

        }
        [Fact]
        public async Task ObtenerCasaPorId_Ok()
        {
            //Arrange
            var casa = new Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1",

            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCasaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(casa);

            var controller = new CasaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<CasaDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CasaDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Casa 1", querydto.Nombre);
        }
        [Fact]
        public async Task ObtenerCasaPorId_Error()
        {
            var error = ErroresCasa.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCasaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new CasaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerCasa_Error()
        {
            var error = ErroresCasa.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCasasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new CasaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerCasaPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();            
            var controller = new CasaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostCasa_DevuelveCreated()
        {
            var casaDto = new CasaDto()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"

            };

            var casa = new Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"
            };



            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCasaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(casa);

            var controller = new CasaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(casaDto);

            var okResult = Assert.IsType<CreatedODataResult<CasaDto>>(result);
            var resultCasa = Assert.IsType<CasaDto>(okResult.Value);
            Assert.Equal("Casa 1", resultCasa.Nombre);
        }
        [Fact]
        public async Task PostCasa_DevuelveBadRequest()
        {
            var casaDto = new CasaDto()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"

            };

            var casa = new Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"
            };            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCasaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(casa);            

            var controller = new CasaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(casaDto);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PostCasa_DevuelveProblem()
        {
            var casaDto = new CasaDto()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"

            };

            var error = Error.Failure();            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCasaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);
            

            var controller = new CasaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(casaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
        [Fact]
        public async Task PatchCasa_InvalidModel()
        {

            var mockSender = new Mock<ISender>();            

            var controller = new CasaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<CasaDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchCasa_Ok()
        {
            var casa = new Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"
            };

            var casaDto = new CasaDto()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"
            };

            var delta = new Delta<CasaDto>(casaDto.GetType());
            delta.TrySetPropertyValue(nameof(casaDto.Nombre), "Casa 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarCasaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(casa, casa));

            var controller = new CasaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchCasa_NotFound()
        {            
            var error = ErroresCasa.NoEncontrada;

            var paisDto = new CasaDto()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"
            };

            var delta = new Delta<CasaDto>(paisDto.GetType());
            delta.TrySetPropertyValue(nameof(paisDto.Nombre), "Casa 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarCasaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new CasaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task DeleteCasa_NotFound()
        {
            var error = ErroresCasa.NoEncontrada;         
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarCasaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);         

            var controller = new CasaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteCasa_Ok()
        {
            var grupo = new Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarCasaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(grupo);

            var controller = new CasaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCasa_InvalidModel()
        {           

            var mockSender = new Mock<ISender>();            
            
            var controller = new CasaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarCasas_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCasasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new CasaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarCasas_DevuelveError()
        {           

            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCasasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());            

            var controller = new CasaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarCasas_DevuelveErrorModeloInvalido()
        {           

            var controller = new CasaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", new List<ImportarCasaDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CasaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
                new CultivoDto { Id = 1, Nombre = "Nombre 1", NombreCientifico = "N1" },
                new CultivoDto { Id = 2, Nombre = "Nombre 2", NombreCientifico = "N2" }
            };

            IEnumerable<ImportarCasaDto> casas = objetos.Cast<ImportarCasaDto>();            

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", casas);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CasaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarCasaDto>());            

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());           

            var controller = new CasaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new CasaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarCasaDto { Nombre = "Casa", Codigo = "999" }
            };

            IEnumerable<ImportarCasaDto> casas = objetos.Cast<ImportarCasaDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", casas);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CasaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }
    }
}
