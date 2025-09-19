using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Pais.Queries.ObtenerPaises;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.Pais.Queries.ObtenerPaisPorId;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.CrearPais;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.EditarPais;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.EliminarPais;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.ImportarDatos;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.EliminarPaises;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;


namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class PaisesControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerPaises_OK()
        {
            //Arange
            var lstPaises = new List<Pais>()
            {
                new Pais()
                {
                     Id = 1,
                     Nombre = "Venezuela",
                     CodigoC3 = "VEN",
                     CodigoA2 = "VE",
                     CodigoNumerico = "123"

                },
                new Pais()
                {
                     Id = 2,
                     Nombre = "España",
                     CodigoC3 = "ESP",
                     CodigoA2 = "ES",
                     CodigoNumerico = "123"

                }
                
            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerPaisesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstPaises);
            
            var controller = new PaisesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<PaisDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<PaisDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Venezuela", querydto[0].Nombre);            
            
        }

        [Fact]
        public async Task ObtenerPaisPorId_Ok()
        {
            //Arrange
            var pais = new Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerPaisPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pais);

            var controller = new PaisesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<PaisDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<PaisDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Venezuela", querydto.Nombre);
        }

        [Fact]
        public async Task ObtenerPaisPorId_Error()
        {
            var error = ErroresPaises.NoEncontrado;           

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerPaisPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new PaisesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task ObtenerPaisPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();            
            var controller = new PaisesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostPais_DevuelveCreated()
        {
            var paisDto = new PaisDto()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var pais = new Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };
            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearPaisCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(pais);

            var controller = new PaisesController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(paisDto);

            var okResult = Assert.IsType<CreatedODataResult<PaisDto>>(result);
            var resultPais = Assert.IsType<PaisDto>(okResult.Value);
            Assert.Equal("Venezuela", resultPais.Nombre);            
        }

        [Fact]
        public async Task PostPais_DevuelveBadRequest()
        {           

            var paisDto = new PaisDto()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var pais = new Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearPaisCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(pais);            

            var controller = new PaisesController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(paisDto);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostPais_DevuelveProblem()
        {
            var paisDto = new PaisDto()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };            

            var pais = Error.Failure();

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearPaisCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(pais);           

            var controller = new PaisesController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(paisDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task PatchPais_InvalidModel()
        {            

            var mockSender = new Mock<ISender>();           

            var controller = new PaisesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<PaisDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchPais_Ok()
        {
            var pais = new Pais()
            {
                Id = 32,
                Nombre = "Pais b",
                CodigoC3 = "PAB",
                CodigoA2 = "PA",
                CodigoNumerico = "123"
            };
            
            var paisDto = new PaisDto()
            {
                Id = 32,
                Nombre = "Pais a",
                CodigoC3 = "PAA",
                CodigoA2 = "PA",
                CodigoNumerico = "123"
            };

            var delta = new Delta<PaisDto>(paisDto.GetType());
            delta.TrySetPropertyValue(nameof(paisDto.Nombre), "Pais b");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarPaisCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(pais, pais));

            var controller = new PaisesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchPais_NotFound()
        {
            var error = ErroresPaises.NoEncontrado;            

            var paisDto = new PaisDto()
            {
                Id = 32,
                Nombre = "Pais a",
                CodigoC3 = "PAA",
                CodigoA2 = "PA",
                CodigoNumerico = "123"
            };

            var delta = new Delta<PaisDto>(paisDto.GetType());
            delta.TrySetPropertyValue(nameof(paisDto.Nombre), "Pais b");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarPaisCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new PaisesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeletePais_NotFound()
        {
            var error = ErroresPaises.NoEncontrado;            

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarPaisCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new PaisesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeletePais_Ok()
        {
            var pais = new Pais()
            {
                Id = 32,
                Nombre = "Pais b",
                CodigoC3 = "PAB",
                CodigoA2 = "PA",
                CodigoNumerico = "123"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarPaisCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(pais);

            var controller = new PaisesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeletePais_InvalidModel()
        {
            var mockSender = new Mock<ISender>();            

            var controller = new PaisesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", new List<ImportarPaisDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new PaisesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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

            IEnumerable<ImportarPaisDto> paises = objetos.Cast<ImportarPaisDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", paises);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);            

            var controller = new PaisesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarPaisDto>());            

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());           

            var controller = new PaisesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            
            var controller = new PaisesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("Modo", "El campo Modo es requerido");

            //Act
            var result = await controller.ImportarDatos(new ODataActionParameters());

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarPaises_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarPaisesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new PaisesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarPaises_DevuelveError()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });           

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarPaisesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());            

            var controller = new PaisesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarPaises_DevuelveErrorModeloInvalido()
        {
            var controller = new PaisesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_FicheroDatosNullError()
        {
            //Arrange
            IEnumerable<object> objetos = new List<object>
            {
                new ImportarPaisDto { Nombre = "Vzla", CodigoA2 = "12", CodigoC3 = "123", CodigoNumerico = "12345" }
            };

            IEnumerable<ImportarPaisDto> paises = objetos.Cast<ImportarPaisDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", paises);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new PaisesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }
    }
}
