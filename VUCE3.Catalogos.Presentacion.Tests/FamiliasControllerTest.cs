using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Familia.Queries.ObtenerFamilias;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.Familia.Queries.ObtenerFamiliasPorId;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.CrearFamilia;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.EditarFamilia;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.EliminarFamilia;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.ImportarDatos;
using VUCE3.Catalogos.Dominio.Errores;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.EliminarFamilias;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using AutoMapper;
using VUCE3.Catalogos.Presentacion.Validadores;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class FamiliasControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerFamilias_OK()
        {
            //Arange
            var lstFamilias = new List<Familia>()
            {
                new Familia()
                {
                    Id =1,
                    Nombre = "Familia 1",

                },
                new Familia()
                {
                    Id =2,
                    Nombre = "Familia 2",

                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerFamiliasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstFamilias);

            var controller = new FamiliaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<FamiliaDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<FamiliaDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Familia 1", querydto[0].Nombre);

        }
        [Fact]
        public async Task ObtenerFamiliaPorId_Ok()
        {
            //Arrange
            var Familia = new Familia()
            {
                Id = 1,
                Nombre = "Familia 1",

            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerFamiliaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Familia);

            var controller = new FamiliaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<FamiliaDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<FamiliaDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Familia 1", querydto.Nombre);
        }
        [Fact]
        public async Task ObtenerFamiliaPorId_Error()
        {
            var error = ErroresFamilia.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerFamiliaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new FamiliaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerFamilia_Error()
        {
            var error = ErroresFamilia.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerFamiliasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new FamiliaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerFamiliaPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();            
            var controller = new FamiliaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostFamilia_DevuelveCreated()
        {
            var FamiliaDto = new FamiliaDto()
            {
                Id = 1,
                Nombre = "Familia 1"

            };

            var Familia = new Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };



            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearFamiliaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Familia);

            var controller = new FamiliaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(FamiliaDto);

            var okResult = Assert.IsType<CreatedODataResult<FamiliaDto>>(result);
            var resultFamilia = Assert.IsType<FamiliaDto>(okResult.Value);
            Assert.Equal("Familia 1", resultFamilia.Nombre);
        }
        [Fact]
        public async Task PostFamilia_DevuelveBadRequest()
        {
            var FamiliaDto = new FamiliaDto()
            {
                Id = 1,
                Nombre = "Familia 1"

            };

            var Familia = new Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearFamiliaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Familia);            

            var controller = new FamiliaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(FamiliaDto);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PostFamilia_DevuelveProblem()
        {
            var FamiliaDto = new FamiliaDto()
            {
                Id = 1,
                Nombre = "Familia 1"

            };

            var error = Error.Failure();            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearFamiliaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);
            

            var controller = new FamiliaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(FamiliaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
        [Fact]
        public async Task PatchFamilia_InvalidModel()
        {

            var mockSender = new Mock<ISender>();            

            var controller = new FamiliaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<FamiliaDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchFamilia_Ok()
        {
            var Familia = new Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            var FamiliaDto = new FamiliaDto()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            var delta = new Delta<FamiliaDto>(FamiliaDto.GetType());
            delta.TrySetPropertyValue(nameof(FamiliaDto.Nombre), "Familia 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarFamiliaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(Familia, Familia));

            var controller = new FamiliaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchFamilia_NotFound()
        {            
            var error = ErroresFamilia.NoEncontrada;

            var paisDto = new FamiliaDto()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            var delta = new Delta<FamiliaDto>(paisDto.GetType());
            delta.TrySetPropertyValue(nameof(paisDto.Nombre), "Familia 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarFamiliaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new FamiliaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task DeleteFamilia_NotFound()
        {
            var error = ErroresFamilia.NoEncontrada;         
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarFamiliaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);         

            var controller = new FamiliaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteFamilia_Ok()
        {
            var grupo = new Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarFamiliaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(grupo);

            var controller = new FamiliaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteFamilia_InvalidModel()
        {           

            var mockSender = new Mock<ISender>();            
            
            var controller = new FamiliaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarFamilias_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarFamiliasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new FamiliaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarFamilias_DevuelveError()
        {           

            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarFamiliasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());            

            var controller = new FamiliaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarFamilias_DevuelveErrorModeloInvalido()
        {           

            var controller = new FamiliaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
            param.Add("datos", new List<ImportarFamiliaDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new FamiliaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
                new { a = 1, b = "Nombre 3" },
                new { a = 2, b = "Nombre 4" }
            };

            IEnumerable<ImportarFamiliaDto> familias = objetos.Cast<ImportarFamiliaDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", familias);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new FamiliaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarFamiliaDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new FamiliaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new FamiliaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarFamiliaDto { Nombre = "Familia Prueba" }
            };

            IEnumerable<ImportarFamiliaDto> familias = objetos.Cast<ImportarFamiliaDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", familias);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new FamiliaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }
       
    }
}
