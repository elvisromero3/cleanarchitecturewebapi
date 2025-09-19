using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.CrearAduana;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EditarAduana;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EliminaAduana;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EliminarAduanas;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Aduanas.Queries.ObtenerAduanaPorId;
using VUCE3.Catalogos.Aplicacion.Aduanas.Queries.ObtenerAduanas;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Tests.Helper;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class AduanasControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerAduanas_Ok()
        {
            //Arrange
            var lstAduanas = new List<Aduana>()
            {
                new Aduana{
                    Id = 1,
                    Nombre = "Aduana 1"                    
                },
                new Aduana{
                    Id = 2,
                    Nombre = "Aduana 2"
                },
            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerAduanasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstAduanas);

            var controller = new AduanasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<AduanaDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<AduanaDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Aduana 1", querydto[0].Nombre);
        }

        [Fact]
        public async Task ObtenerAduanas_Error()
        {
            var error = Error.Failure();
            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();            
            mockMediator.Setup(s => s.Send(It.IsAny<ObtenerAduanasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new AduanasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("500", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task ObtenerAduanaPorId_Ok()
        {
            //Arrange
            var aduana = new Aduana
            {
                Id = 1,
                Nombre = "Aduana 1"
            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerAduanaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(aduana);

            var controller = new AduanasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<AduanaDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<AduanaDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Aduana 1", querydto.Nombre);
        }

        

        [Fact]
        public async Task ObtenerAduanaPorId_Error()
        {
            var error = ErroresAduana.NoEncontrada;
            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerAduanaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new AduanasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task ObtenerAduanaPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new AduanasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostDivision_Ok()
        {
            var aduana = new Aduana
            {
                Id = 1,
                Nombre = "Aduana 1"
            };

            var aduanaDto = new AduanaDto
            {
                Id = 1,
                Nombre = "Aduana 1"
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearAduanaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(aduana);

            var controller = new AduanasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(aduanaDto);

            var okResult = Assert.IsType<CreatedODataResult<AduanaDto>>(result);
            var resultDivision = Assert.IsType<AduanaDto>(okResult.Value);
            Assert.Equal("Aduana 1", resultDivision.Nombre);
            Assert.Equal(1, resultDivision.Id);
        }

        [Fact]
        public async Task PostAduana_Error()
        {
            var aduanaDto = new AduanaDto
            {
                Id = 1,
                Nombre = "Aduana 1"
            };

            var aduana = Error.Failure();            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearAduanaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(aduana);            

            var controller = new AduanasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(aduanaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
            
        }

        [Fact]
        public async Task PostAduana_ErrorModelInvalid()
        {
            var aduana = new Aduana
            {
                Id = 1,
                Nombre = "Aduana 1"
            };

            var aduanaDto = new AduanaDto
            {
                Id = 1,
                Nombre = "Aduana 1"
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearAduanaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(aduana);

            var controller = new AduanasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(aduanaDto);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchAduana_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new AduanasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<AduanaDto>());
            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PatchAduana_Ok()
        {
            var aduana = new Aduana()
            {
                Id = 32,
                Nombre = "Aduana a"
            };

            var aduanaDto = new AduanaDto()
            {
                Id = 32,
                Nombre = "Aduana a"
            };

            var delta = new Delta<AduanaDto>(aduanaDto.GetType());
            delta.TrySetPropertyValue(nameof(aduanaDto.Nombre), "Aduana a");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarAduanaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(aduana, aduana));

            var controller = new AduanasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchAduana_NotFound()
        {
            var error = ErroresAduana.NoEncontrada;

            var aduanaDto = new AduanaDto()
            {
                Id = 32,
                Nombre = "Aduana a"
            };
            

            var delta = new Delta<AduanaDto>(aduanaDto.GetType());
            delta.TrySetPropertyValue(nameof(aduanaDto.Nombre), "Aduana a");

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarAduanaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new AduanasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task DeleteAduana_NotFound()
        {
            var error = ErroresAduana.NoEncontrada;
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarAduanaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new AduanasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
            
        }
        [Fact]
        public async Task DeleteAduana_Ok()
        {
            var aduana = new Aduana()
            {
                Id = 32,
                Nombre = "Aduana a"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarAduanaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(aduana);

            var controller = new AduanasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteAduana_InvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new AduanasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
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
            param.Add("datos", new List<ImportarAduanaDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new AduanasController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

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
                new CultivoDto { Id = 1, Nombre = "Nombre 3", NombreCientifico = "N3" },
                new CultivoDto { Id = 2, Nombre = "Nombre 4", NombreCientifico = "N4" }
            };

            IEnumerable<ImportarAduanaDto> aduanas = objetos.Cast<ImportarAduanaDto>();            

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", aduanas);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);           

            var controller = new AduanasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarAduanaDto>());            

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());           

            var controller = new AduanasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new AduanasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("Modo", "El campo Modo es requerido");

            //Act
            var result = await controller.ImportarDatos(new ODataActionParameters());

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarMasivamenteAduanas_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarAduanasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new AduanasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarMasivamenteAduanas_DevuelveError()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });           

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarAduanasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());           

            var controller = new AduanasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarMasivamenteAduanas_DevuelveErrorModeloInvalido()
        {
            var controller = new AduanasController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);
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
                new ImportarAduanaDto { Nombre = "Aduana Prueba" }
            };

            IEnumerable<ImportarAduanaDto> aduanas = objetos.Cast<ImportarAduanaDto>();            

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", aduanas);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new AduanasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }
    }
}
