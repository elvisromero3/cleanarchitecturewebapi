using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Queries.ObtenerCaracteristicaTipoProductos;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Queries.ObtenerCaracteristicaTipoProductosPorId;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.CrearCaracteristicaTipoProducto;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EditarCaracteristicaTipoProducto;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EliminarCaracteristicaTipoProducto;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EliminarCaracteristicaTipoProductos;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.ImportarDatos;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Dominio.Errores;
using Microsoft.AspNetCore.OData.Formatter;

using VUCE3.Catalogos.Presentacion.Resources;
using AutoMapper;
using VUCE3.Catalogos.Presentacion.Validadores;
using System.Reflection.PortableExecutable;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.EliminarFamilias;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class CaracteristicaTipoProductosControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerCaracteristicaTipoProductos_OK()
        {
            //Arange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1, IdCategoria = 1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var caracteristicaTipoProducto = new List<Dominio.Entidades.CaracteristicaTipoProducto>()
            {
                new Dominio.Entidades.CaracteristicaTipoProducto()
                {
                    Id =1,
                   TipoProducto = tipoproducto,
                   Caracteristica=caracteristica,
                }
            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCaracteristicaTipoProductoQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(caracteristicaTipoProducto);

            var controller = new CaracteristicaTipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<CaracteristicaTipoProductoDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<CaracteristicaTipoProductoDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
           

        }
        [Fact]
        public async Task ObtenerCaracteristicaTipoProductoPorId_Ok()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1, IdCategoria = 1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var caracteristicaTipoProducto =
                new Dominio.Entidades.CaracteristicaTipoProducto()
                {
                    Id =1,
                    TipoProducto = tipoproducto,
                    Caracteristica=caracteristica,

                };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCaracteristicaTipoProductoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(caracteristicaTipoProducto);

            var controller = new CaracteristicaTipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<CaracteristicaTipoProductoDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CaracteristicaTipoProductoDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
           
        }
        [Fact]
        public async Task ObtenerCaracteristicaTipoProductoPorId_Error()
        {
            var error = ErroresCaracteristicaTipoProducto.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCaracteristicaTipoProductoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new CaracteristicaTipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerCaracteristicaTipoProducto_Error()
        {
            var error = ErroresCaracteristicaTipoProducto.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCaracteristicaTipoProductoQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new CaracteristicaTipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerCaracteristicaTipoProductoPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();            
            var controller = new CaracteristicaTipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostCaracteristicaTipoProducto_DevuelveCreated()
        {
            var CaracteristicaTipoProductoDto = new CaracteristicaTipoProductoDto()
            {
                Id =1,
                IdTipoProducto =1,
                IdCaracteristica =1


            };

            var caracteristicaTipoProducto =
                 new Dominio.Entidades.CaracteristicaTipoProducto()
                 {
                     Id =1,
                     IdTipoProducto =1,
                     IdCaracteristica =1

                 };



            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCaracteristicaTipoProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(caracteristicaTipoProducto);

            var controller = new CaracteristicaTipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(CaracteristicaTipoProductoDto);

            var okResult = Assert.IsType<CreatedODataResult<CaracteristicaTipoProductoDto>>(result);
            var resultCaracteristicaTipoProducto = Assert.IsType<CaracteristicaTipoProductoDto>(okResult.Value);
            Assert.Equal(1, resultCaracteristicaTipoProducto.Id);
        }
        [Fact]
        public async Task PostCaracteristicaTipoProducto_DevuelveBadRequest()
        {
            var CaracteristicaTipoProductoDto = new CaracteristicaTipoProductoDto()
            {
                Id = 1,
                IdTipoProducto =1,
                IdCaracteristica =1

            };

            var CaracteristicaTipoProducto = new CaracteristicaTipoProducto()
            {
                Id = 1,
                IdTipoProducto =1,
                IdCaracteristica =1
            };            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCaracteristicaTipoProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(CaracteristicaTipoProducto);            

            var controller = new CaracteristicaTipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(CaracteristicaTipoProductoDto);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PostCaracteristicaTipoProducto_DevuelveProblem()
        {
            var CaracteristicaTipoProductoDto = new CaracteristicaTipoProductoDto()
            {
                Id = 1,
                IdTipoProducto =1,
                IdCaracteristica =1

            };

            var error = Error.Failure();            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCaracteristicaTipoProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);
            

            var controller = new CaracteristicaTipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(CaracteristicaTipoProductoDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
        [Fact]
        public async Task PatchCaracteristicaTipoProducto_InvalidModel()
        {

            var mockSender = new Mock<ISender>();            

            var controller = new CaracteristicaTipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<CaracteristicaTipoProductoDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchCaracteristicaTipoProducto_Ok()
        {
            var CaracteristicaTipoProducto = new CaracteristicaTipoProducto()
            {
                Id = 1,
                IdTipoProducto =1,
                IdCaracteristica =1
            };

            var CaracteristicaTipoProductoDto = new CaracteristicaTipoProductoDto()
            {
                Id = 1,
                IdTipoProducto =1,
                IdCaracteristica =1
            };

            var delta = new Delta<CaracteristicaTipoProductoDto>(CaracteristicaTipoProductoDto.GetType());
            delta.TrySetPropertyValue(nameof(CaracteristicaTipoProductoDto.Id), 1);

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarCaracteristicaTipoProductoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(CaracteristicaTipoProducto, CaracteristicaTipoProducto));

            var controller = new CaracteristicaTipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchCaracteristicaTipoProducto_NotFound()
        {            
            var error = ErroresCaracteristicaTipoProducto.NoEncontrada;

            var paisDto = new CaracteristicaTipoProductoDto()
            {
                Id = 1,
                IdTipoProducto =1,
                IdCaracteristica =1
            };

            var delta = new Delta<CaracteristicaTipoProductoDto>(paisDto.GetType());
            delta.TrySetPropertyValue(nameof(paisDto.Id), 1);

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarCaracteristicaTipoProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new CaracteristicaTipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task DeleteCaracteristicaTipoProducto_NotFound()
        {
            var error = ErroresCaracteristicaTipoProducto.NoEncontrada;         
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarCaracteristicaTipoProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);         

            var controller = new CaracteristicaTipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteCaracteristicaTipoProducto_Ok()
        {
            var grupo = new CaracteristicaTipoProducto()
            {
                Id = 1,
                IdTipoProducto =1,
                IdCaracteristica =1
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarCaracteristicaTipoProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(grupo);

            var controller = new CaracteristicaTipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCaracteristicaTipoProducto_InvalidModel()
        {           

            var mockSender = new Mock<ISender>();            
            
            var controller = new CaracteristicaTipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task BorradoMasivoCaracteristicaTipoProductoOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCaracteristicaTipoProductosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new CaracteristicaTipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task BorradoMasivoCaracteristicaTipoProducto_DevuelveError()
        {

            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCaracteristicaTipoProductosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new CaracteristicaTipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task BorradoMasivoCaracteristicaTipoProducto_DevuelveErrorModeloInvalido()
        {

            var controller = new CaracteristicaTipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            var datos = new List<ImportarCaracteristicaTipoProductoDto>
            {
                new ImportarCaracteristicaTipoProductoDto { Caracteristica = "Caracteristica 1", IdTipoProducto = 1 },
                new ImportarCaracteristicaTipoProductoDto { Caracteristica = "Caracteristica 2", IdTipoProducto = 2 }
            };
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", datos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CaracteristicaTipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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

            IEnumerable<ImportarCaracteristicaTipoProductoDto> caracteristicastipoproducto = objetos.Cast<ImportarCaracteristicaTipoProductoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", caracteristicastipoproducto);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CaracteristicaTipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarCaracteristicaTipoProductoDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new CaracteristicaTipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new CaracteristicaTipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarCaracteristicaTipoProductoDto {Caracteristica = "Caracteristica 1" }
            };

            IEnumerable<ImportarCaracteristicaTipoProductoDto> familias = objetos.Cast<ImportarCaracteristicaTipoProductoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", familias);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CaracteristicaTipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }

    }
}
