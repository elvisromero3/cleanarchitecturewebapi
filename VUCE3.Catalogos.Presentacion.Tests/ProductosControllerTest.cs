using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Productos.Queries.ObtenerProductos;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.Productos.Queries.ObtenerProductosPorId;
using VUCE3.Catalogos.Aplicacion.Productos.Commands.CrearProductos;
using VUCE3.Catalogos.Aplicacion.Productos.Commands.EditarProductos;
using VUCE3.Catalogos.Aplicacion.Productos.Commands.EliminarProducto;
using VUCE3.Catalogos.Aplicacion.Productos.Commands.EliminarProductos;
using VUCE3.Catalogos.Dominio.Errores;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Aplicacion.Productos.Commands.ImportarDatos;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class ProductosControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerProductos_OK()
        {
            //Arange
            var lstProductos = new List<Productos>()
            {
                new Productos()
                {

                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional=true

                },
                new Productos()
                {

                    Id = 2,
                    Clase = "Clase 2",
                    Presentacion = "Presentacion 2",
                    NombreComun = "Nombre Comun 2",
                    NombreCientifico = "Nombre Cientifico 2",
                    Tradicional=true
                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProductosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstProductos);

            var controller = new ProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<ProductosDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<ProductosDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Nombre Comun 1", querydto[0].NombreComun);

        }
        [Fact]
        public async Task ObtenerProductosPorId_Ok()
        {
            //Arrange
            var Productos = new Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true

            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProductosPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Productos);

            var controller = new ProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<ProductosDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ProductosDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Nombre Comun 1", querydto.NombreComun);
        }
        [Fact]
        public async Task ObtenerProductosPorId_Error()
        {
            var error = ErroresProductos.NoEncontrado;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProductosPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new ProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }
        [Fact]
        public async Task ObtenerProductos_Error()
        {
            var error = ErroresProductos.NoEncontrado;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProductosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new ProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }
        [Fact]
        public async Task ObtenerProductosPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();
            var controller = new ProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostProductos_DevuelveCreated()
        {
            var ProductosDto = new ProductosDto()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true

            };

            var Productos = new Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };



            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearProductosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Productos);

            var controller = new ProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(ProductosDto);

            var okResult = Assert.IsType<CreatedODataResult<ProductosDto>>(result);
            var resultProductos = Assert.IsType<ProductosDto>(okResult.Value);
            Assert.Equal("Nombre Comun 1", resultProductos.NombreComun);
        }
        [Fact]
        public async Task PostProductos_DevuelveBadRequest()
        {
            var ProductosDto = new ProductosDto()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true

            };

            var Productos = new Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearProductosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Productos);

            var controller = new ProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(ProductosDto);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PostProductos_DevuelveProblem()
        {
            var ProductosDto = new ProductosDto()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true

            };

            var error = Error.Failure();

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearProductosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);


            var controller = new ProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(ProductosDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
        [Fact]
        public async Task PatchProductos_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new ProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<ProductosDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchProductos_Ok()
        {
            var Productos = new Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            var ProductosDto = new ProductosDto()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            var delta = new Delta<ProductosDto>(ProductosDto.GetType());
            delta.TrySetPropertyValue(nameof(ProductosDto.NombreComun), "Nombre Comun 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarProductosCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(Productos, Productos));

            var controller = new ProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchProductos_NotFound()
        {
            var error = ErroresProductos.NoEncontrado;

            var productioDto = new ProductosDto()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            var delta = new Delta<ProductosDto>(productioDto.GetType());
            delta.TrySetPropertyValue(nameof(productioDto.NombreComun), "Nombre Comun 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarProductosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new ProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }

        [Fact]
        public async Task DeleteProductos_NotFound()
        {
            var error = ErroresProductos.NoEncontrado;
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new ProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task DeleteProductos_Ok()
        {
            var grupo = new Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(grupo);

            var controller = new ProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteProductos_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new ProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            IEnumerable<ImportarProductoDto> productoDtos = new List<ImportarProductoDto>
            {
                new ImportarProductoDto
                {
                    Clase = "Clase1",
                    Presentacion = "Presentacion1",
                    NombreComun = "NombreComun",
                    NombreCientifico = "NombreCientifico",
                    Tradicional = true
                }
            };
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", productoDtos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ProductosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task ImpoterDatos_FicheroEstructuraError()
        {
            //Arrange
            IEnumerable<object> objects = new List<object>
            {
                new ProductosDto
                {
                    Id = 1,
                    Clase = "Clase1",
                    Presentacion = "Presentacion1",
                    NombreComun = "NombreComun",
                    NombreCientifico = "NombreCientifico",
                    Tradicional = true
                },
                new ProductosDto
                {
                    Id = 2,
                    Clase = "Clase2",
                    Presentacion = "Presentacion2",
                    NombreComun = "NombreComun",
                    NombreCientifico = "NombreCientifico",
                    Tradicional = true
                }
            };

            IEnumerable<ImportarProductoDto> bloques = objects.Cast<ImportarProductoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", bloques);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ProductosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportadDatos_DevuelveError()
        {
            //Arrange
            IEnumerable<ImportarProductoDto> productoDtos = new List<ImportarProductoDto>
            {
                new ImportarProductoDto
                {
                    Clase = "Clase1",
                    Presentacion = "Presentacion1",
                    NombreComun = "NombreComun",
                    NombreCientifico = "NombreCientifico",
                    Tradicional = true
                }
            };
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", productoDtos);
            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());
            var controller = new ProductosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

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
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", new List<object>());
            var controller = new ProductosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");
            //Act
            var result = await controller.ImportarDatos(param);
            //Assert
            Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("400", ((ODataError)Assert.IsType<ODataErrorResult>(result).Error).ErrorCode);
        }

        [Fact]
        public async Task ImportarDatos_FicheroDatosNullError()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", null);
            var controller = new ProductosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            //Act
            var result = await controller.ImportarDatos(param);
            //Assert
            Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("400", ((ODataError)Assert.IsType<ODataErrorResult>(result).Error).ErrorCode);
        }

        [Fact]
        public async Task BorradoMasivo_DevuelveOk()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });
            mockMediator.Setup(m => m.Send(It.IsAny<EliminarProductosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);
            var controller = new ProductosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task BorradoMasivo_ErrorModeliInvalido()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });
            var controller = new ProductosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");
            //Act
            var result = await controller.BorradoMasivo(param);
            //Assert
            Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("400", ((ODataError)Assert.IsType<ODataErrorResult>(result).Error).ErrorCode);
        }

        [Fact]
        public async Task BorradoMasivo_Error()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });
            mockMediator.Setup(m => m.Send(It.IsAny<EliminarProductosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());
            var controller = new ProductosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            
            //Act
            var result = await controller.BorradoMasivo(param);
            
            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
    }
}
