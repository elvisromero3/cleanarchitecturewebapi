using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.CrearNoticiasVuce;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EditarNoticiasVuce;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EliminarNoticiasVuce;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Queries.ObtenerNoticiasVucePorId;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Queries.ObtenerNoticiasVuce;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EliminarMasivoNoticiasVuce;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class NoticiasVuceControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();

        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerNoticiasVuce_OK()
        {
            //Arange
            var lstNoticiasVuce = new List<NoticiasVuce>()
            {
                new NoticiasVuce ()
                {
                    Id =1,
                    Titulo ="Noticia Vuce Titulo",
                    Texto= "Noticia Vuce TExto",
                    Enlace ="Enlace Vuce",
                    TextoIngles = "News Vuce Text",
                    TituloIngles ="News Vue Title"
                    
                },
                new NoticiasVuce ()
                {
                    Id =2,
                    Titulo ="Noticia Vuce Titulo 2",
                    Texto= "Noticia Vuce TExto 2 ",
                    Enlace ="Enlace Vuce 2 ",
                    TextoIngles = "News Vuce Text 2",
                    TituloIngles ="News Vue Title 2"

                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerNoticiasVuceQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstNoticiasVuce);

            var controller = new NoticiasVuceController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<NoticiasVuceDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<NoticiasVuceDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Noticia Vuce Titulo", querydto[0].Titulo);            
            
        }
        [Fact]
        public async Task ObtenerNoticiasVuce_Error()
        {
            var error = Error.Failure();            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerNoticiasVuceQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new NoticiasVuceController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("500", errorRes.Error.ErrorCode);
         
        }

        [Fact]
        public async Task ObtenerNoticiasVucePorId_Ok()
        {
            //Arrange
            var noticiasVuce = new NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerNoticiasVucePorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(noticiasVuce);

            var controller = new NoticiasVuceController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<NoticiasVuceDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<NoticiasVuceDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Noticia Vuce Titulo", querydto.Titulo);
        }

        [Fact]
        public async Task ObtenerNoticiaVucePorId_Error()
        {
            var error = ErroresNoticiasVuce.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerNoticiasVucePorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);
            

            var controller = new NoticiasVuceController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task ObtenerNoticiasVucePorId_InvalidModel()
        {           

            var mockSender = new Mock<ISender>();

            var controller = new NoticiasVuceController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostNoticiasVuce_DevuelveCreated()
        {
            var noticiasVuceDto = new NoticiasVuceDto()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            var noticiasVuce = new NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };



            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearNoticiasVuceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(noticiasVuce);

            var controller = new NoticiasVuceController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(noticiasVuceDto);

            var okResult = Assert.IsType<CreatedODataResult<NoticiasVuceDto>>(result);
            var resultNoticiasVuce = Assert.IsType<NoticiasVuceDto>(okResult.Value);
            Assert.Equal("Noticia Vuce Titulo", resultNoticiasVuce.Titulo);            
        }

        [Fact]
        public async Task PostNoticiasVuce_DevuelveBadRequest()
        {            

            var noticiasVuceDto = new NoticiasVuceDto()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            var noticiasVuce = new NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };


            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearNoticiasVuceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(noticiasVuce);            

            var controller = new NoticiasVuceController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(noticiasVuceDto);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostNoticiaVuce_DevuelveProblem()
        {
            var noticiasVuceDto = new NoticiasVuceDto()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };            

            var error = Error.Failure();

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearNoticiasVuceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new NoticiasVuceController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(noticiasVuceDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task PatchNoticiasVuce_InvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new NoticiasVuceController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<NoticiasVuceDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchNoticiasVuce_Ok()
        {
            var noticiasVuceDto = new NoticiasVuceDto()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            var noticiasVuce = new NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            var delta = new Delta<NoticiasVuceDto>(noticiasVuceDto.GetType());
            delta.TrySetPropertyValue(nameof(noticiasVuceDto.Titulo), "Noticia Vuce Titulo 2");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarNoticiasVuceCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(noticiasVuce, noticiasVuce));

            var controller = new NoticiasVuceController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchNoticiasVuce_NotFound()
        {
            var error = ErroresNoticiasVuce.NoEncontrada;           

            var noticiasVuceDto = new NoticiasVuceDto()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            var delta = new Delta<NoticiasVuceDto>(noticiasVuceDto.GetType());
            delta.TrySetPropertyValue(nameof(noticiasVuceDto.Titulo), "Noticia Vuce Titulo b");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarNoticiasVuceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new NoticiasVuceController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteNoticiasVuce_NotFound()
        {
            var error = ErroresNoticiasVuce.NoEncontrada;
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarNoticiasVuceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new NoticiasVuceController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteNoticiasVuce_Ok()
        {
            var noticiasVuce = new NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarNoticiasVuceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(noticiasVuce);

            var controller = new NoticiasVuceController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteNoticiasVuce_InvalidModel()
        {
            var mockSender = new Mock<ISender>();           

            var controller = new NoticiasVuceController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
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
            param.Add("datos", new List<ImportarNoticiasVuceDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new NoticiasVuceController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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

            IEnumerable<ImportarNoticiasVuceDto> noticias = objetos.Cast<ImportarNoticiasVuceDto>();
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", noticias);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new NoticiasVuceController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarNoticiasVuceDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());           

            var controller = new NoticiasVuceController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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

            var controller = new NoticiasVuceController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("Modo", "El campo Modo es requerido");

            //Act
            var result = await controller.ImportarDatos(new ODataActionParameters());

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarMasivamente_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarMasivoNoticiasVuceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new NoticiasVuceController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarMasivamente_DevuelveError()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });            

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarMasivoNoticiasVuceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());            

            var controller = new NoticiasVuceController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarMasivamente_DevuelveErrorModeloInvalido()
        {
            var controller = new NoticiasVuceController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarNoticiasVuceDto { Texto = "Texto", Enlace = "Link", TextoIngles = "Texto Ingles", Titulo = "Titulo", TituloIngles = "Titulo Ingles" }
            };

            IEnumerable<ImportarNoticiasVuceDto> noticias = objetos.Cast<ImportarNoticiasVuceDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", noticias);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new NoticiasVuceController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }
    }
}
