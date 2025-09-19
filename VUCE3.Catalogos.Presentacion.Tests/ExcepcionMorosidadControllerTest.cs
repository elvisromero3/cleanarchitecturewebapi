using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.CrearExcepcionMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EditarExcepcionMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EliminarExcepcionesMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EliminarExcepcionMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Queries.ObtenerExcepcionesMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Queries.ObtenerExcepcionMorosidadPorId;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Tests.Helper;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class ExcepcionMorosidadControllerTest
    {
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerExcepcionesMorosidad_OK()
        {
            //Arange
            var lstExcepcionMorosidad = new List<ExcepcionMorosidad>()
            {
                new ExcepcionMorosidad()
                    {
                        Id = 1
                    },
                new ExcepcionMorosidad()
                    {
                        Id = 2
                    }
            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerExcepcionesMorosidadQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstExcepcionMorosidad);

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<ExcepcionMorosidadDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<ExcepcionMorosidadDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);

        }

        [Fact]
        public async Task ObtenerExcepcionesMorosidad_Error()
        {
            //Arange
            var lstExcepcionMorosidad = new List<ExcepcionMorosidad>()
            {
                new ExcepcionMorosidad()
                    {
                        Id = 1
                    },
                new ExcepcionMorosidad()
                    {
                        Id = 2
                    }
            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerExcepcionesMorosidadQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("500", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerExcepcionMorosidadPorId_Ok()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1
            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerExcepcionMorosidadPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(excepcion);

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<ExcepcionMorosidadDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ExcepcionMorosidadDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);

        }

        [Fact]
        public async Task ObtenerExcepcionMorosidadPorId_Error()
        {
            var error = ErroresExcepcionMorosidad.NoEncontrada;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerExcepcionMorosidadPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }

        [Fact]
        public async Task ObtenerExcepcionMorosidadPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();
            var controller = new ExcepcionMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostExcepcionMorosidad_DevuelveCreated()
        {
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1
            };

            var excepcionDto = new ExcepcionMorosidadDto()
            {
                Id = 1
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearExcepcionMorosidadCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(excepcion);

            var controller = new ExcepcionMorosidadController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(excepcionDto);

            var okResult = Assert.IsType<CreatedODataResult<ExcepcionMorosidadDto>>(result);
            var resultExcepcionMorosidad = Assert.IsType<ExcepcionMorosidadDto>(okResult.Value);
            Assert.Equal(1, resultExcepcionMorosidad.Id);
        }

        [Fact]
        public async Task PostExcepcionMorosidadPorId_InvalidModel()
        {
            var excepcionDto = new ExcepcionMorosidadDto()
            {
                Id = 1
            };

            var mockSender = new Mock<ISender>();
            var controller = new ExcepcionMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Post(excepcionDto);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostExcepcionMorosidad_DevuelveProblem()
        {
            var excepcionDto = new ExcepcionMorosidadDto()
            {
                Id = 1
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearExcepcionMorosidadCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new ExcepcionMorosidadController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(excepcionDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task PatchExcepcionMorosidad_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<ExcepcionMorosidadDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchExcepcionMorosidad_Ok()
        {
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1
            };

            var excepcionDto = new ExcepcionMorosidadDto()
            {
                Id = 1
            };

            var delta = new Delta<ExcepcionMorosidadDto>(excepcionDto.GetType());
            delta.TrySetPropertyValue(nameof(excepcionDto.Observaciones), "test");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarExcepcionMorosidadCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(excepcion, excepcion));

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchExcepcionMorosidad_NotFound()
        {
            var error = ErroresExcepcionMorosidad.NoEncontrada;

            var excepcionDto = new ExcepcionMorosidadDto()
            {
                Id = 1
            };

            var delta = new Delta<ExcepcionMorosidadDto>(excepcionDto.GetType());
            delta.TrySetPropertyValue(nameof(excepcionDto.Observaciones), "test");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarExcepcionMorosidadCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }

        [Fact]
        public async Task DeleteExcepcionMorosidad_NotFound()
        {
            var error = ErroresExcepcionMorosidad.NoEncontrada;
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarExcepcionMorosidadCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task DeleteExcepcionMorosidad_Ok()
        {
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarExcepcionMorosidadCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(excepcion);

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteExcepcionMorosidad_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarMasivamenteExcepcionesMorosidad_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            var mockSender = new Mock<ISender>();
            mockSender.Setup(m => m.Send(It.IsAny<EliminarExcepcionesMorosidadCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarMasivamenteExcepcionesMorosidad_DevuelveErrorModeloInvalido()
        {
            var mockSender = new Mock<ISender>();
            var controller = new ExcepcionMorosidadController(mockSender.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarMasivamenteExcepcionesMorosidad_Error()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            var mockSender = new Mock<ISender>();
            mockSender.Setup(m => m.Send(It.IsAny<EliminarExcepcionesMorosidadCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            var dto = new ImportarExcepcionMorosidadDto
            {
                Tramite = 10,
                TipoTramite = 20,
                Regimen = 30,
                TipoAccion = 40,
                TipoIdentificacion = 'F',
                NumeroIdentificacion = "999999999",
                FechaInicio = "2025-06-12",
                FechaVencimiento = "2026-12-31",
                Empresa = "Empresa Test",
                Observaciones = "Observación test"
            };

            var param = new ODataActionParameters
            {
                { "modo", 1 },
                { "datos", new List<ImportarExcepcionMorosidadDto> { dto } }
            };

            var mockSender = new Mock<ISender>();
            mockSender.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
            mockSender.Verify(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveErrorModeloInvalido()
        {
            var mockSender = new Mock<ISender>();

            //Arrange
            var controller = new ExcepcionMorosidadController(mockSender.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("Modo", "El campo Modo es requerido");

            //Act
            var result = await controller.ImportarDatos(new ODataActionParameters());

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_FicheroEstructuraError()
        {
            var mockSender = new Mock<ISender>();

            //Arrange
            IEnumerable<object> objetos = new List<object>
            {
                new CultivoDto { Id = 1, Nombre = "Nombre 3", NombreCientifico = "N3" },
                new CultivoDto { Id = 2, Nombre = "Nombre 4", NombreCientifico = "N4" }
            };

            IEnumerable<ImportarExcepcionMorosidadDto> excepciones = objetos.Cast<ImportarExcepcionMorosidadDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", excepciones);

            mockSender.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveError()
        {
            var mockSender = new Mock<ISender>();
            //Arrange
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", new List<ImportarExcepcionMorosidadDto>());

            mockSender.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ImportarDatos_FicheroDatosNullError()
        {
            var mockSender = new Mock<ISender>();
            //Arrange
            IEnumerable<object> objetos = new List<object>
            {
                new ImportarExcepcionMorosidadDto {Empresa = "Nombre Prueba" }
            };

            IEnumerable<ImportarExcepcionMorosidadDto> excepciones = objetos.Cast<ImportarExcepcionMorosidadDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", excepciones);

            mockSender.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_FicheroFechaInicioError()
        {
            var mockSender = new Mock<ISender>();

            //Arrange
            IEnumerable<object> objetos = new List<object>
            {
                new ImportarExcepcionMorosidadDto
                {
                    Tramite = 1,
                    TipoTramite = 1,
                    Regimen = 1,
                    TipoAccion = 1,
                    TipoIdentificacion = 'F',
                    NumeroIdentificacion = "123456789",
                    FechaInicio = "2026341",
                    FechaVencimiento = "2028-12-31",
                    Empresa = "Nombre empresa",
                    Observaciones = "Observaciones"
                }
            };

            IEnumerable<ImportarExcepcionMorosidadDto> excepciones = objetos.Cast<ImportarExcepcionMorosidadDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", excepciones);

            mockSender.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_FicheroFechaVencimientoError()
        {
            var mockSender = new Mock<ISender>();

            //Arrange
            IEnumerable<object> objetos = new List<object>
            {
                new ImportarExcepcionMorosidadDto
                {
                    Tramite = 1,
                    TipoTramite = 1,
                    Regimen = 1,
                    TipoAccion = 1,
                    TipoIdentificacion = 'F',
                    NumeroIdentificacion = "123456789",
                    FechaInicio = "2028-12-31",
                    FechaVencimiento = "2028aaa",
                    Empresa = "Nombre empresa",
                    Observaciones = "Observaciones"
                }
            };

            IEnumerable<ImportarExcepcionMorosidadDto> excepciones = objetos.Cast<ImportarExcepcionMorosidadDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", excepciones);

            mockSender.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ExcepcionMorosidadController(mockSender.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        
    }
}
