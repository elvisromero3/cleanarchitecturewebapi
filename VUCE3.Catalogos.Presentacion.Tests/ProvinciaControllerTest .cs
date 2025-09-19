using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Provincia.Queries.ObtenerProvincias;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.Provincia.Queries.ObtenerProvinciaPorId;
using VUCE3.Catalogos.Aplicacion.Provincia.Commands.CrearProvincia;
using VUCE3.Catalogos.Aplicacion.Provincia.Commands.EditarProvincia;
using VUCE3.Catalogos.Aplicacion.Provincia.Commands.EliminarProvincia;
using VUCE3.Catalogos.Dominio.Errores;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Aplicacion.Provincias.Commands.EliminarProvincias;
using VUCE3.Catalogos.Aplicacion.Provincias.Commands.ImportarDatos;


namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class ProvinciasControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerProvincias_OK()
        {
            //Arange
            var lstProvincias = new List<Provincia>()
            {
                new Provincia()
                {
                    Id =1,
                    Nombre = "Provincia 1",

                },
                new Provincia()
                {
                    Id =2,
                    Nombre = "Provincia 2",

                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProvinciasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstProvincias);

            var controller = new ProvinciasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<ProvinciaDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<ProvinciaDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Provincia 1", querydto[0].Nombre);

        }
        [Fact]
        public async Task ObtenerProvinciasPorId_Ok()
        {
            //Arrange
            var provincia = new Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",

            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProvinciaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(provincia);

            var controller = new ProvinciasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<ProvinciaDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ProvinciaDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Provincia 1", querydto.Nombre);
        }
        [Fact]
        public async Task ObtenerProvinciaPorId_Error()
        {
            var error = ErroresProvincia.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProvinciaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new ProvinciasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerProvincia_Error()
        {
            var error = ErroresProvincia.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProvinciasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new ProvinciasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerProvinciaPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();            
            var controller = new ProvinciasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostProvincia_DevuelveCreated()
        {
            var provinciaDto = new ProvinciaDto()
            {
                Id = 1,
                Nombre = "Provincia 1"

            };

            var provincia = new Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1"
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearProvinciaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(provincia);

            var controller = new ProvinciasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(provinciaDto);

            var okResult = Assert.IsType<CreatedODataResult<ProvinciaDto>>(result);
            var resultProvincia = Assert.IsType<ProvinciaDto>(okResult.Value);
            Assert.Equal("Provincia 1", resultProvincia.Nombre);
        }
        [Fact]
        public async Task PostProvincia_DevuelveBadRequest()
        {
            var provinciaDto = new ProvinciaDto()
            {
                Id = 1,
                Nombre = "Provincia 1"

            };

            var provincia = new Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1"
            };            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearProvinciaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(provincia);            

            var controller = new ProvinciasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(provinciaDto);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PostProvincia_DevuelveProblem()
        {
            var provinciaDto = new ProvinciaDto()
            {
                Id = 1,
                Nombre = "Provincia 1"

            };

            var error = Error.Failure();            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearProvinciaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);
            

            var controller = new ProvinciasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(provinciaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
        [Fact]
        public async Task PatchProvincia_InvalidModel()
        {

            var mockSender = new Mock<ISender>();            

            var controller = new ProvinciasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<ProvinciaDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchProvincia_Ok()
        {
            var provincia = new Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1"
            };

            var provinciaDto = new ProvinciaDto()
            {
                Id = 1,
                Nombre = "Provincia 1"
            };

            var delta = new Delta<ProvinciaDto>(provinciaDto.GetType());
            delta.TrySetPropertyValue(nameof(provinciaDto.Nombre), "Provincia 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarProvinciaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(provincia, provincia));

            var controller = new ProvinciasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchProvincia_NotFound()
        {            
            var error = ErroresProvincia.NoEncontrada;

            var provinciaDto = new ProvinciaDto()
            {
                Id = 1,
                Nombre = "Provincia 1"
            };

            var delta = new Delta<ProvinciaDto>(provinciaDto.GetType());
            delta.TrySetPropertyValue(nameof(provinciaDto.Nombre), "Provincia 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarProvinciaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new ProvinciasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task DeleteProvincia_NotFound()
        {
            var error = ErroresProvincia.NoEncontrada;         
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarProvinciaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);         

            var controller = new ProvinciasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteProvincia_Ok()
        {
            var provincia = new Provincia()
            {
                Id = 1,                
                Nombre = "Provincia 1"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarProvinciaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(provincia);

            var controller = new ProvinciasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteProvincia_InvalidModel()
        {           

            var mockSender = new Mock<ISender>();            
            
            var controller = new ProvinciasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarProvincias_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarProvinciasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new ProvinciasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarProvincias_DevuelveError()
        {

            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarProvinciasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new ProvinciasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarProvincias_DevuelveErrorModeloInvalido()
        {

            var controller = new ProvinciasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            IEnumerable<ImportarProvinciaDto> objetos = new List<ImportarProvinciaDto>
            {
                new ImportarProvinciaDto
                {
                    CodigoProvincia = "01",
                    Provincia = "Nombre1"
                }
            };

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", objetos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ProvinciasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
                new ProvinciaDto { Id = 1, Codigo = "01", Nombre = "Nombre 1" },
                new ProvinciaDto { Id = 2, Codigo = "02", Nombre = "Nombre 2" }
            };

            IEnumerable<ImportarProvinciaDto> bloques = objetos.Cast<ImportarProvinciaDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", bloques);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ProvinciasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarProvinciaDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new ProvinciasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new ProvinciasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarProvinciaDto {CodigoProvincia="01", Provincia = "" }
            };

            IEnumerable<ImportarProvinciaDto> bloques = objetos.Cast<ImportarProvinciaDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", bloques);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ProvinciasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }
    }
}
