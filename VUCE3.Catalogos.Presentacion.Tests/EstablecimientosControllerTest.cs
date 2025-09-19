using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EliminarEstablecimientos;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.CrearEstablecimiento;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EditarEstablecimiento;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EliminarEstablecimiento;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Queries.ObtenerEstablecimientoPorId;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Queries.ObtenerEstablecimientos;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.ImportarDatos;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Aplicacion.Casa.Queries.ObtenerCasas;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class EstablecimientosControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
         
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerEstablecimientos_Ok()
        {
            //Arrange
            var establecimientos = new List<Establecimiento>
            {
                new Establecimiento
                {
                    Id = 1,
                    NumeroCvo = "123456",
                    ActividadPrimaria = "Actividad1",
                    ActividadSecundaria = "Actividad2",
                    Provincia = "Provincia1",
                    Canton = "Canton1",
                    FechaVencimiento = DateTime.Now.AddDays(5),
                     EstadoEstablecimiento = "Activo"
                },
                new Establecimiento
                {
                    Id = 2,
                    NumeroCvo = "654321",
                    ActividadPrimaria = "Actividad1",
                    ActividadSecundaria = "Actividad2",
                    Provincia = "Provincia1",
                    Canton = "Canton1",
                    FechaVencimiento = DateTime.Now.AddDays(3),
                    EstadoEstablecimiento = "Activo"
                }
            };

            mockMediator.Setup(x => x.Send(It.IsAny<ObtenerEstablecimientosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(establecimientos);

            var controller = new EstablecimientosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Get();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<List<EstablecimientoDto>>(okResult.Value);
            Assert.Equal(establecimientos.Count, model.Count);
        }

        [Fact]
        public async Task ObtenerEstablecimientos_Error()
        {
            var error = ErroresEstablecimiento.NoEncontrado;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerEstablecimientosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new EstablecimientosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerEstablecimientoPorId_Ok()
        {
            //Arrange
            var establecimiento = new Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                ActividadPrimaria = "Actividad1",
                ActividadSecundaria = "Actividad2",
                Provincia = "Provincia1",
                Canton = "Canton1",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo"
            };
            mockMediator.Setup(x => x.Send(It.IsAny<ObtenerEstablecimientosPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(establecimiento);
            var controller = new EstablecimientosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            
            //Act
            var result = await controller.Get(1);
            
            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EstablecimientoDto>(okResult.Value);
            Assert.Equal(establecimiento.Id, model.Id);
        }

        [Fact]
        public async Task ObtenerEstablecimientoPorId_ErrorNoEncontrado()
        {
            //Arrange
            mockMediator.Setup(x => x.Send(It.IsAny<ObtenerEstablecimientosPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ErroresEstablecimiento.NoEncontrado);
            var controller = new EstablecimientosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Get(2);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerEstablecimientoPorId_ErrorModeloInvalido()
        {
            //Arrange
            var controller = new EstablecimientosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            //Act
            var result = await controller.Get(1);
            
            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("400", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task Post_Ok()
        {
            //Arrange
            var establecimiento = new Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                ActividadPrimaria = "Actividad1",
                ActividadSecundaria = "Actividad2",
                Provincia = "Provincia1",
                Canton = "Canton1",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo"
            };
            mockMediator.Setup(x => x.Send(It.IsAny<CrearEstablecimientoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(establecimiento);
            var controller = new EstablecimientosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Post(new EstablecimientoDto
            {
                NumeroCvo = "123456",
                ActividadPrimaria = "Actividad1",
                ActividadSecundaria = "Actividad2",
                Provincia = "Provincia1",
                Canton = "Canton1",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo"
            });

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EstablecimientoDto>(okResult.Value);
            Assert.Equal(establecimiento.Id, model.Id);
        }

        [Fact]
        public async Task Post_ErrorModeloInvalido()
        {
            //Arrange
            var controller = new EstablecimientosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");
            //Act
            var result = await controller.Post(new EstablecimientoDto());
            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("400", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task Post_Error()
        {
            //Arrange
            mockMediator.Setup(x => x.Send(It.IsAny<CrearEstablecimientoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ErroresEstablecimiento.NumeroCvoInvalido);
            var controller = new EstablecimientosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            //Act
            var result = await controller.Post(new EstablecimientoDto
            {
                NumeroCvo = "",
                ActividadPrimaria = "Actividad1",
                ActividadSecundaria = "Actividad2",
                Provincia = "Provincia1",
                Canton = "Canton1",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo"
            });
            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("409", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task Patch_Ok()
        {
            //Arrange
            var establecimiento = new Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                ActividadPrimaria = "Actividad1",
                ActividadSecundaria = "Actividad2",
                Provincia = "Provincia1",
                Canton = "Canton1",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo"
            };

            var establecimientoDto = new EstablecimientoDto
            {
                Id = 1,
                NumeroCvo = "123456",
                ActividadPrimaria = "Actividad1",
                ActividadSecundaria = "Actividad2",
                Provincia = "Provincia1",
                Canton = "Canton1",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo"
            };

            var delta = new Delta<EstablecimientoDto>(establecimientoDto.GetType());
            delta.TrySetPropertyValue("NumeroCvo", "123456");

            mockMediator.Setup(x => x.Send(It.IsAny<EditarEstablecimientoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(establecimiento, establecimiento));

            var controller = new EstablecimientosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Patch(1, delta);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Patch_ErrorModeloInvalido()
        {
            //Arrange
            var controller = new EstablecimientosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");
            //Act
            var result = await controller.Patch(1, new Delta<EstablecimientoDto>(typeof(EstablecimientoDto)));
            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("400", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task Patch_Error()
        {
            //Arrange
            mockMediator.Setup(x => x.Send(It.IsAny<EditarEstablecimientoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ErroresEstablecimiento.NumeroCvoInvalido);
            var controller = new EstablecimientosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            //Act
            var result = await controller.Patch(1, new Delta<EstablecimientoDto>(typeof(EstablecimientoDto)));
            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("409", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task Delete_Ok()
        {
            //Arrange
            var establecimiento = new Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                ActividadPrimaria = "Actividad1",
                ActividadSecundaria = "Actividad2",
                Provincia = "Provincia1",
                Canton = "Canton1",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo"
            };

            mockMediator.Setup(x => x.Send(It.IsAny<EliminarEstablecimientoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(establecimiento);
            var controller = new EstablecimientosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            //Act
            var result = await controller.Delete(1);
            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ErrorModeloInvalido()
        {
            //Arrange
            var controller = new EstablecimientosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");
            //Act
            var result = await controller.Delete(1);
            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("400", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task Delete_ErrorNoEncontrado()
        {
            //Arrange
            mockMediator.Setup(x => x.Send(It.IsAny<EliminarEstablecimientoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ErroresEstablecimiento.NoEncontrado);
            var controller = new EstablecimientosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            //Act
            var result = await controller.Delete(1);
            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task EliminarEstablecimientos_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarEstablecimientosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new EstablecimientosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarEstablecimientos_DevuelveError()
        {

            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarEstablecimientosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new EstablecimientosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarEstablecimientos_DevuelveErrorModeloInvalido()
        {

            var controller = new EstablecimientosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
            param.Add("datos", new List<ImportarEstablecimientoDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new EstablecimientosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
                new Establecimiento {Id=1,  NumeroCvo = "123456",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(5) },
                new Establecimiento { Id = 2,  NumeroCvo = "123456",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(5)}
            };

            IEnumerable<ImportarEstablecimientoDto> Establecimientos = objetos.Cast<ImportarEstablecimientoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", Establecimientos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new EstablecimientosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarEstablecimientoDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new EstablecimientosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new EstablecimientosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarEstablecimientoDto { NumeroCvo = "",
                    NombreEstablecimiento ="",
                    ActividadPrimaria = "",
                    ActividadSecundaria = "",
                    Provincia = "",
                    Canton = "",
                    Distrito = "",
                    DireccionExacta = "",
                    FechaVencimiento = "2025-01-01",
                    EstadoEstablecimiento = "Activo"
                },
            };

            IEnumerable<ImportarEstablecimientoDto> Establecimientos = objetos.Cast<ImportarEstablecimientoDto>();
        
            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", Establecimientos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new EstablecimientosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
           
            //Act
            var result = await controller.ImportarDatos(param);
            var errorRes = Assert.IsType<ODataErrorResult>(result);

            //Assert
            Assert.Equal("400", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task ImportarDatos_FechaVencimientoNull()
        {
            //Arrange
            IEnumerable<object> objetos = new List<object>
            {
                new ImportarEstablecimientoDto { 
                    NumeroCvo = "123",
                    NombreEstablecimiento ="abc",
                    ActividadPrimaria = "actividad",
                    ActividadSecundaria = null,
                    Provincia = "provincia",
                    Canton = "canton",
                    Distrito = "distrito",
                    DireccionExacta = "dirección",
                    EstadoEstablecimiento = "Activo"
                },
            };

            IEnumerable<ImportarEstablecimientoDto> Establecimientos = objetos.Cast<ImportarEstablecimientoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", Establecimientos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new EstablecimientosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);
            var errorRes = Assert.IsType<ODataErrorResult>(result);

            //Assert
            Assert.Equal("409", errorRes.Error.ErrorCode);
        }
    }
}
