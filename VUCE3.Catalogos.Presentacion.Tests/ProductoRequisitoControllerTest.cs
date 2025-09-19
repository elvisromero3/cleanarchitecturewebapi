using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Queries.ObtenerProductoRequisitos;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Queries.ObtenerProductoRequisitosPorId;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.CrearProductoRequisito;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EditarProductoRequisito;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EliminarProductoRequisito;

using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.ImportarDatos.DTO;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Dominio.Errores;
using Microsoft.AspNetCore.OData.Formatter;

using VUCE3.Catalogos.Presentacion.Resources;
using AutoMapper;
using VUCE3.Catalogos.Presentacion.Validadores;
using System.Reflection.PortableExecutable;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisitos;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EliminarProductoRequisitos;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class ProductoRequisitosControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerProductoRequisitos_OK()
        {
            //Arange

            var productoRequisito = new List<Dominio.Entidades.ProductoRequisito>()
            {
                new Dominio.Entidades.ProductoRequisito()
                {
                   Id =1,
                   IdRequisito=1,
                   IdTipoProducto=1
                }
            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProductoRequisitoQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(productoRequisito);

            var controller = new ProductoRequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<ProductoRequisitoDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<ProductoRequisitoDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
           

        }
        [Fact]
        public async Task ObtenerProductoRequisitoPorId_Ok()
        {
            //Arrange

            var productoRequisito =
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                    IdRequisito=1,
                    IdTipoProducto=1

                };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProductoRequisitoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(productoRequisito);

            var controller = new ProductoRequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<ProductoRequisitoDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ProductoRequisitoDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
           
        }
        [Fact]
        public async Task ObtenerProductoRequisitoPorId_Error()
        {
            var error = ErroresProductoRequisito.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProductoRequisitoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new ProductoRequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerProductoRequisito_Error()
        {
            var error = ErroresProductoRequisito.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProductoRequisitoQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new ProductoRequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerProductoRequisitoPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();            
            var controller = new ProductoRequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostProductoRequisito_DevuelveCreated()
        {
            var ProductoRequisitoDto = new ProductoRequisitoDto()
            {
                Id =1,
                IdRequisito=1,
                IdTipoProducto=1

            };

            var productoRequisito =
                 new Dominio.Entidades.ProductoRequisito()
                 {
                     Id =1,
                     IdRequisito=1,
                     IdTipoProducto=1

                 };



            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearProductoRequisitoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(productoRequisito);

            var controller = new ProductoRequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(ProductoRequisitoDto);

            var okResult = Assert.IsType<CreatedODataResult<ProductoRequisitoDto>>(result);
            var resultProductoRequisito = Assert.IsType<ProductoRequisitoDto>(okResult.Value);
            Assert.Equal(1, resultProductoRequisito.Id);
        }
        [Fact]
        public async Task PostProductoRequisito_DevuelveBadRequest()
        {
            var ProductoRequisitoDto = new ProductoRequisitoDto()
            {
                Id = 1,
                IdRequisito=1,
                IdTipoProducto=1

            };

            var ProductoRequisito = new ProductoRequisito()
            {
                Id = 1,
                IdRequisito=1,
                IdTipoProducto=1
            };            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearProductoRequisitoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(ProductoRequisito);            

            var controller = new ProductoRequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(ProductoRequisitoDto);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PostProductoRequisito_DevuelveProblem()
        {
            var ProductoRequisitoDto = new ProductoRequisitoDto()
            {
                Id = 1,
                IdRequisito=1,
                IdTipoProducto=1

            };

            var error = Error.Failure();            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearProductoRequisitoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);
            

            var controller = new ProductoRequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(ProductoRequisitoDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
        [Fact]
        public async Task PatchProductoRequisito_InvalidModel()
        {

            var mockSender = new Mock<ISender>();            

            var controller = new ProductoRequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<ProductoRequisitoDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchProductoRequisito_Ok()
        {
            var ProductoRequisito = new ProductoRequisito()
            {
                Id = 1,
                IdRequisito=1,
                IdTipoProducto=1
            };

            var ProductoRequisitoDto = new ProductoRequisitoDto()
            {
                Id = 1,
                IdRequisito=1,
                IdTipoProducto=1
            };

            var delta = new Delta<ProductoRequisitoDto>(ProductoRequisitoDto.GetType());
            delta.TrySetPropertyValue(nameof(ProductoRequisitoDto.Id), 1);

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarProductoRequisitoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(ProductoRequisito, ProductoRequisito));

            var controller = new ProductoRequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchProductoRequisito_NotFound()
        {            
            var error = ErroresProductoRequisito.NoEncontrada;

            var paisDto = new ProductoRequisitoDto()
            {
                Id = 1,
                IdRequisito=1,
                IdTipoProducto=1
            };

            var delta = new Delta<ProductoRequisitoDto>(paisDto.GetType());
            delta.TrySetPropertyValue(nameof(paisDto.Id), 1);

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarProductoRequisitoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new ProductoRequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task DeleteProductoRequisito_NotFound()
        {
            var error = ErroresProductoRequisito.NoEncontrada;         
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarProductoRequisitoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);         

            var controller = new ProductoRequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteProductoRequisito_Ok()
        {
            var grupo = new ProductoRequisito()
            {
                Id = 1,
                IdRequisito=1,
                IdTipoProducto=1
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarProductoRequisitoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(grupo);

            var controller = new ProductoRequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteProductoRequisito_InvalidModel()
        {           

            var mockSender = new Mock<ISender>();            
            
            var controller = new ProductoRequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task BorradoMasivoProductoRequisitoOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarProductoRequisitosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new ProductoRequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }


        [Fact]
        public async Task BorradoMasivoRequisito_DevuelveError()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarProductoRequisitosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new ProductoRequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task BorradoMasivoRequisito_DevuelveErrorModeloInvalido()
        {

            var controller = new ProductoRequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            var datos = new List<ImportarProductoRequisitoDto>
            {
                 new ImportarProductoRequisitoDto
                {
                    Categoria="Categoria 1", TipoProducto="Tipo 1", IdRequisito =1
                },
                new ImportarProductoRequisitoDto
                {
                    Categoria="Categoria 2", TipoProducto="Tipo 2", IdRequisito =2
                }};
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", datos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ProductoRequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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

            IEnumerable<ImportarProductoRequisitoDto> requisito = objetos.Cast<ImportarProductoRequisitoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", requisito);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarProductoRequisitoDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ProductoRequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarProductoRequisitoDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new ProductoRequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new ProductoRequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarProductoRequisitoCommandDto
                {
                     Categoria="Categoria 1", TipoProducto="Tipo 1", IdRequisito =1
                }
            };

            IEnumerable<ImportarProductoRequisitoDto> familias = objetos.Cast<ImportarProductoRequisitoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", familias);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ProductoRequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }





    }
}
