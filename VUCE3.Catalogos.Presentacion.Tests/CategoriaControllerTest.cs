using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EliminarAduanas;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.CrearCategoria;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.EditarCategoria;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.EliminarCategoria;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.EliminarCategorias;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.ImportarDatos;

using VUCE3.Catalogos.Aplicacion.Categoria.Queries.ObtenerCategoriaPorId;
using VUCE3.Catalogos.Aplicacion.Categoria.Queries.ObtenerCategorias;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Tests.Helper;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class CategoriaControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerCategorias_OK()
        {
            //Arange
            var lstCategorias = new List<Categoria>()
            {
                new Categoria()
                {
                    Id =1,
                    Nombre = "Categoria 1",

                },
                new Categoria()
                {
                    Id =2,
                    Nombre = "Categoria 2",

                }
            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCategoriasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstCategorias);

            var controller = new CategoriaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<CategoriaDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<CategoriaDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Categoria 1", querydto[0].Nombre);

        }
        [Fact]
        public async Task ObtenerCategoriaPorId_Ok()
        {
            //Arrange
            var Categoria = new Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCategoriaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Categoria);

            var controller = new CategoriaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<CategoriaDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CategoriaDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Categoria 1", querydto.Nombre);
        }
        [Fact]
        public async Task ObtenerCategoriaPorId_Error()
        {
            var error = ErroresCategoria.NoEncontrada;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCategoriaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new CategoriaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }
        [Fact]
        public async Task ObtenerCategoria_Error()
        {
            var error = ErroresCategoria.NoEncontrada;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCategoriasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new CategoriaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }
        [Fact]
        public async Task ObtenerCategoriaPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();
            var controller = new CategoriaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostCategoria_DevuelveCreated()
        {
            var CategoriaDto = new CategoriaDto()
            {
                Id = 1,
                Nombre = "Categoria 1"
            };

            var Categoria = new Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1"
            };



            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCategoriaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Categoria);

            var controller = new CategoriaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(CategoriaDto);

            var okResult = Assert.IsType<CreatedODataResult<CategoriaDto>>(result);
            var resultCategoria = Assert.IsType<CategoriaDto>(okResult.Value);
            Assert.Equal("Categoria 1", resultCategoria.Nombre);
        }
        [Fact]
        public async Task PostCategoria_DevuelveBadRequest()
        {
            var CategoriaDto = new CategoriaDto()
            {
                Id = 1,
                Nombre = "Categoria 1"

            };

            var Categoria = new Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1"
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCategoriaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Categoria);

            var controller = new CategoriaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(CategoriaDto);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PostCategoria_DevuelveProblem()
        {
            var CategoriaDto = new CategoriaDto()
            {
                Id = 1,
                Nombre = "Categoria 1"

            };

            var error = Error.Failure();

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearCategoriaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);


            var controller = new CategoriaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(CategoriaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
        [Fact]
        public async Task PatchCategoria_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new CategoriaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<CategoriaDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchCategoria_Ok()
        {
            var Categoria = new Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1"
            };

            var CategoriaDto = new CategoriaDto()
            {
                Id = 1,
                Nombre = "Categoria 1"
            };

            var delta = new Delta<CategoriaDto>(CategoriaDto.GetType());
            delta.TrySetPropertyValue(nameof(CategoriaDto.Nombre), "Categoria 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarCategoriaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(Categoria, Categoria));

            var controller = new CategoriaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchCategoria_NotFound()
        {
            var error = ErroresCategoria.NoEncontrada;

            var paisDto = new CategoriaDto()
            {
                Id = 1,
                Nombre = "Categoria 1"
            };

            var delta = new Delta<CategoriaDto>(paisDto.GetType());
            delta.TrySetPropertyValue(nameof(paisDto.Nombre), "Categoria 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarCategoriaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new CategoriaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }

        [Fact]
        public async Task DeleteCategoria_NotFound()
        {
            var error = ErroresCategoria.NoEncontrada;
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarCategoriaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new CategoriaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task DeleteCategoria_Ok()
        {
            var grupo = new Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarCategoriaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(grupo);

            var controller = new CategoriaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCategoria_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new CategoriaController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
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
            param.Add("datos", new List<ImportarCategoriaDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CategoriaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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

            IEnumerable<ImportarCategoriaDto> familias = objetos.Cast<ImportarCategoriaDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", familias);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CategoriaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarCategoriaDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new CategoriaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new CategoriaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarCategoriaDto { Nombre = "Categoria Prueba" , Institucion="MINCEX"}
            };

            IEnumerable<ImportarCategoriaDto> familias = objetos.Cast<ImportarCategoriaDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", familias);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CategoriaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }
        [Fact]
        public async Task EliminarMasivamenteCategorias_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCategoriasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new CategoriaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarMasivamenteCategorias_DevuelveError()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCategoriasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new CategoriaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarMasivamenteCategorias_DevuelveErrorModeloInvalido()
        {
            var controller = new CategoriaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }
    }
}
