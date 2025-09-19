using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Queries.ObtenerBloquesComerciales;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Queries.ObtenerBloqueComercialPorId;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.CrearBloqueComercial;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.EditarBloqueComercial;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.EliminarBloqueComercial;
using VUCE3.Catalogos.Dominio.Errores;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Aplicacion.BloquesComerciales.Commands.EliminarBloquesComerciales;
using VUCE3.Catalogos.Aplicacion.BloquesComerciales.Commands.Importardatos;


namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class BloquesComercialesControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerBloquesComerciales_OK()
        {
            //Arange
            var lstBloquesComerciales = new List<BloqueComercial>()
            {
                new BloqueComercial()
                {
                    Id =1,
                    Nombre = "BloqueComercial 1",

                },
                new BloqueComercial()
                {
                    Id =2,
                    Nombre = "BloqueComercial 2",

                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerBloquesComercialesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstBloquesComerciales);

            var controller = new BloquesComercialesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<BloqueComercialDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<BloqueComercialDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("BloqueComercial 1", querydto[0].Nombre);

        }
        [Fact]
        public async Task ObtenerBloquesComercialesPorId_Ok()
        {
            //Arrange
            var provincia = new BloqueComercial()
            {
                Id = 1,
                Nombre = "BloqueComercial 1",

            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerBloqueComercialPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(provincia);

            var controller = new BloquesComercialesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<BloqueComercialDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BloqueComercialDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("BloqueComercial 1", querydto.Nombre);
        }
        [Fact]
        public async Task ObtenerBloqueComercialPorId_Error()
        {
            var error = ErroresBloqueComercial.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerBloqueComercialPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new BloquesComercialesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerBloqueComercial_Error()
        {
            var error = ErroresBloqueComercial.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerBloquesComercialesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new BloquesComercialesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerBloqueComercialPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();            
            var controller = new BloquesComercialesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostBloqueComercial_DevuelveCreated()
        {
            var provinciaDto = new BloqueComercialDto()
            {
                Id = 1,
                Nombre = "BloqueComercial 1"

            };

            var provincia = new BloqueComercial()
            {
                Id = 1,
                Nombre = "BloqueComercial 1"
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearBloqueComercialCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(provincia);

            var controller = new BloquesComercialesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(provinciaDto);

            var okResult = Assert.IsType<CreatedODataResult<BloqueComercialDto>>(result);
            var resultBloqueComercial = Assert.IsType<BloqueComercialDto>(okResult.Value);
            Assert.Equal("BloqueComercial 1", resultBloqueComercial.Nombre);
        }
        [Fact]
        public async Task PostBloqueComercial_DevuelveBadRequest()
        {
            var provinciaDto = new BloqueComercialDto()
            {
                Id = 1,
                Nombre = "BloqueComercial 1"

            };

            var provincia = new BloqueComercial()
            {
                Id = 1,
                Nombre = "BloqueComercial 1"
            };            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearBloqueComercialCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(provincia);            

            var controller = new BloquesComercialesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(provinciaDto);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PostBloqueComercial_DevuelveProblem()
        {
            var provinciaDto = new BloqueComercialDto()
            {
                Id = 1,
                Nombre = "BloqueComercial 1"

            };

            var error = Error.Failure();            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearBloqueComercialCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);
            

            var controller = new BloquesComercialesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(provinciaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
        [Fact]
        public async Task PatchBloqueComercial_InvalidModel()
        {

            var mockSender = new Mock<ISender>();            

            var controller = new BloquesComercialesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<BloqueComercialDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchBloqueComercial_Ok()
        {
            var provincia = new BloqueComercial()
            {
                Id = 1,
                Nombre = "BloqueComercial 1"
            };

            var provinciaDto = new BloqueComercialDto()
            {
                Id = 1,
                Nombre = "BloqueComercial 1"
            };

            var delta = new Delta<BloqueComercialDto>(provinciaDto.GetType());
            delta.TrySetPropertyValue(nameof(provinciaDto.Nombre), "BloqueComercial 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarBloqueComercialCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(provincia, provincia));

            var controller = new BloquesComercialesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchBloqueComercial_NotFound()
        {            
            var error = ErroresBloqueComercial.NoEncontrada;

            var provinciaDto = new BloqueComercialDto()
            {
                Id = 1,
                Nombre = "BloqueComercial 1"
            };

            var delta = new Delta<BloqueComercialDto>(provinciaDto.GetType());
            delta.TrySetPropertyValue(nameof(provinciaDto.Nombre), "BloqueComercial 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarBloqueComercialCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new BloquesComercialesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task DeleteBloqueComercial_NotFound()
        {
            var error = ErroresBloqueComercial.NoEncontrada;         
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarBloqueComercialCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);         

            var controller = new BloquesComercialesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteBloqueComercial_Ok()
        {
            var provincia = new BloqueComercial()
            {
                Id = 1,                
                Nombre = "BloqueComercial 1"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarBloqueComercialCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(provincia);

            var controller = new BloquesComercialesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteBloqueComercial_InvalidModel()
        {           

            var mockSender = new Mock<ISender>();            
            
            var controller = new BloquesComercialesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarBloquesComerciales_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarBloquesComercialesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new BloquesComercialesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarBloquesComerciales_DevuelveError()
        {

            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarBloquesComercialesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new BloquesComercialesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarBloquesComerciales_DevuelveErrorModeloInvalido()
        {

            var controller = new BloquesComercialesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            IEnumerable<ImportarBloqueComercialDto> objetos = new List<ImportarBloqueComercialDto>
            {
                new ImportarBloqueComercialDto
                {
                    Nombre = "Nombre1"
                }
            };

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", objetos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new BloquesComercialesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
                new BloqueComercialDto { Id = 1, Nombre = "Nombre 1" },
                new BloqueComercialDto { Id = 2, Nombre = "Nombre 2" }
            };

            IEnumerable<ImportarBloqueComercialDto> bloques = objetos.Cast<ImportarBloqueComercialDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", bloques);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new BloquesComercialesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarBloqueComercialDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new BloquesComercialesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new BloquesComercialesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarBloqueComercialDto { Nombre = "" }
            };

            IEnumerable<ImportarBloqueComercialDto> bloques = objetos.Cast<ImportarBloqueComercialDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", bloques);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new BloquesComercialesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

    }
}
