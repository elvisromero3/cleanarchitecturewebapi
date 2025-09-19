using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Queries.ObtenerTipoProductos;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Queries.ObtenerTipoProductoPorId;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.CrearTipoProducto;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.EditarTipoProducto;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.EliminarTipoProducto;
using VUCE3.Catalogos.Dominio.Errores;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.EliminarTipoProductos;
using VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.ImportarDatos.DTO;


namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class TipoProductosControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerTipoProductos_OK()
        {
            //Arange
            var lstTipoProductos = new List<TipoProducto>()
            {
                new TipoProducto()
                {
                    Id =1,
                     IdCategoria=1,
                Tipo = "Tipo 1",
                IdInstitucion = 1

                },
                new TipoProducto()
                {
                    Id =2,
                    IdCategoria=2,
                    Tipo = "Tipo 2",
                    IdInstitucion = 1

                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerTipoProductosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstTipoProductos);

            var controller = new TipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<TipoProductoDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<TipoProductoDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Tipo 1", querydto[0].Tipo);

        }
        [Fact]
        public async Task ObtenerTipoProductosPorId_Ok()
        {
            //Arrange
            var tipoProducto = new TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1

            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerTipoProductoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(tipoProducto);

            var controller = new TipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<TipoProductoDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<TipoProductoDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Tipo 1", querydto.Tipo);
        }
        [Fact]
        public async Task ObtenerTipoProductoPorId_Error()
        {
            var error = ErroresTipoProducto.NoEncontrado;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerTipoProductoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new TipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerTipoProducto_Error()
        {
            var error = ErroresTipoProducto.NoEncontrado;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerTipoProductosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new TipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerTipoProductoPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();            
            var controller = new TipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostTipoProducto_DevuelveCreated()
        {
            var tipoProductoDto = new TipoProductoDto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1

            };

            var tipoProducto = new TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearTipoProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(tipoProducto);

            var controller = new TipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(tipoProductoDto);

            var okResult = Assert.IsType<CreatedODataResult<TipoProductoDto>>(result);
            var resultTipoProducto = Assert.IsType<TipoProductoDto>(okResult.Value);
            Assert.Equal("Tipo 1", resultTipoProducto.Tipo);
        }
        [Fact]
        public async Task PostTipoProducto_DevuelveBadRequest()
        {
            var tipoProductoDto = new TipoProductoDto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1

            };

            var tipoProducto = new TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearTipoProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(tipoProducto);            

            var controller = new TipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(tipoProductoDto);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PostTipoProducto_DevuelveProblem()
        {
            var tipoProductoDto = new TipoProductoDto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1

            };

            var error = Error.Failure();            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearTipoProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);
            

            var controller = new TipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(tipoProductoDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
        [Fact]
        public async Task PatchTipoProducto_InvalidModel()
        {

            var mockSender = new Mock<ISender>();            

            var controller = new TipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<TipoProductoDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchTipoProducto_Ok()
        {
            var tipoProducto = new TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var tipoProductoDto = new TipoProductoDto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var delta = new Delta<TipoProductoDto>(tipoProductoDto.GetType());
            delta.TrySetPropertyValue(nameof(tipoProductoDto.Tipo), "Tipo 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarTipoProductoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(tipoProducto, tipoProducto));

            var controller = new TipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchTipoProducto_NotFound()
        {            
            var error = ErroresTipoProducto.NoEncontrado;

            var tipoProductoDto = new TipoProductoDto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var delta = new Delta<TipoProductoDto>(tipoProductoDto.GetType());
            delta.TrySetPropertyValue(nameof(tipoProductoDto.Tipo), "Tipo 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarTipoProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new TipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task DeleteTipoProducto_NotFound()
        {
            var error = ErroresTipoProducto.NoEncontrado;         
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarTipoProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);         

            var controller = new TipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteTipoProducto_Ok()
        {
            var tipoProducto = new TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarTipoProductoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(tipoProducto);

            var controller = new TipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteTipoProducto_InvalidModel()
        {           

            var mockSender = new Mock<ISender>();            
            
            var controller = new TipoProductosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task BorradoMasivoTipoProductoOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarTipoProductosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new TipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task BorradoMasivoTipoProducto_DevuelveError()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarTipoProductosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new TipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task BorradoMasivoTipoProducto_DevuelveErrorModeloInvalido()
        {

            var controller = new TipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            var datos = new List<ImportarTipoProductoDto>
            {
                 new ImportarTipoProductoDto
                {
                    Categoria = "Categoria 01",
                    Tipo = "Tipo 1",
                    Institucion = "Institucion 1",
                }
                ,
                new ImportarTipoProductoDto
                {
                    Categoria = "Categoria 02",
                    Tipo = "Tipo 1",
                    Institucion = "Institucion 1",
                    }
                };
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", datos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new TipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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

            IEnumerable<ImportarTipoProductoDto> TipoProducto = objetos.Cast<ImportarTipoProductoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", TipoProducto);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new TipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarTipoProductoDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new TipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new TipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarTipoProductoCommandDto
                {
                    Categoria = "Categoria 01",
                    Tipo = "Tipo 1",
                    Institucion = "Institucion 1",
                    }

            };

            IEnumerable<ImportarTipoProductoDto> familias = objetos.Cast<ImportarTipoProductoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", familias);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new TipoProductosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

    }
}
