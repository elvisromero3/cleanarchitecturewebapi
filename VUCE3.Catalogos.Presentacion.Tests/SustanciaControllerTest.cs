using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EditarSustancia;
using VUCE3.Catalogos.Aplicacion.Sustancia.Commands.CrearSustancia;
using VUCE3.Catalogos.Aplicacion.Sustancia.Queries.ObtenerSustanciaPorId;
using VUCE3.Catalogos.Aplicacion.Sustancia.Queries.ObtenerSustancias;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EliminarSustancia;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EliminarSustancias;
using VUCE3.Catalogos.Aplicacion.Sustancia.Commands.ImportarDatos;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class SustanciaControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerSustancias_Ok()
        {
            // Arrange
            var sustancias = new List<Sustancia>
            {
                new Sustancia
                {
                    Id = 1,
                    Nombre = "Sustancia1",
                    Cas = "452125",
                    ListaCaq = "Lista1"
                },
                new Sustancia
                {
                    Id = 2,
                    Nombre = "Sustancia2",
                    Cas = "774589",
                    ListaCaq = "List2"
                },
                new Sustancia
                {
                    Id = 3,
                    Nombre = "Sustancia3",
                    Cas = "56141",
                    ListaCaq = "List3"
                },
            };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerSustanciasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(sustancias);

            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            // Act
            var result = await controller.Get();

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var sustanciasDto = Assert.IsType<List<SustanciaDto>>(objectResult.Value);

            Assert.Equal(sustancias.Count, sustanciasDto.Count);
            for (int i = 0; i < sustancias.Count; i++)
            {
                Assert.Equal(sustancias[i].Id, sustanciasDto[i].Id);
                Assert.Equal(sustancias[i].Nombre, sustanciasDto[i].Nombre);
            }
        }

        //ObtenerSustancias_DevuelveError
        [Fact]
        public async Task ObtenerSustancias_DevuelveError()
        {
            //Arrange
            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerSustanciasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());

            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            // Act
            var result = await controller.Get();

            // Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);

            Assert.Equal("404", problemDetails.ErrorCode);
        }

        //ObtenerSustanciaPorId_DevuelveOk
        [Fact]
        public async Task ObtenerSustanciaPorId_DevuelveOk()
        {
            // Arrange
            var sustancia = new Sustancia
            {
                Id = 1,
                Nombre = "Sustancia1",
                Cas = "452125",
                ListaCaq = "Lista1"
            };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerSustanciaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(sustancia);

            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Get(1);

            //Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var sustanciaDto = Assert.IsType<SustanciaDto>(objectResult.Value);

            Assert.Equal(sustancia.Id, sustanciaDto.Id);
            Assert.Equal(sustancia.Nombre, sustanciaDto.Nombre);
            Assert.Equal(sustancia.Cas, sustanciaDto.Cas);
            Assert.Equal(sustancia.ListaCaq, sustanciaDto.ListaCaq);
        }

        //ObtenerSustanciaPorId_DevuelveError
        [Fact]
        public async Task ObtenerSustanciaPorId_DevuelveError()
        {
            //Arrange
            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerSustanciaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());

            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Get(1);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        //ObtenerSustanciaPorId_DevuelveErrorModeloInvalido
        [Fact]
        public async Task ObtenerSustanciaPorId_DevuelveErrorModeloInvalido()
        {
            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("IdVariedad", "El campo IdVariedad es requerido");

            var result = await controller.Get(0);

            Assert.IsType<ODataErrorResult>(result);
        }

        //CrearSustancia_DevuelveOk
        [Fact]
        public async Task CrearSustancia_DevuelveOk()
        {
            // Arrange
            var sustancia = new Sustancia
            {
                Id = 1,
                Nombre = "Sustancia1",
                Cas = "452125",
                ListaCaq = "Lista1"
            };

            mockMediator.Setup(m => m.Send(It.IsAny<CrearSustanciaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(sustancia);

            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Post(new SustanciaDto
            {
                Id = 1,
                Nombre = "Sustancia1",
                Cas = "452125",
                ListaCaq = "Lista1"
            });

            Assert.IsType<CreatedODataResult<SustanciaDto>>(result);
        }

        //CrearSustancia_DevuelveError
        [Fact]
        public async Task CrearSustancia_DevuelveError()
        {
            mockMediator.Setup(m => m.Send(It.IsAny<CrearSustanciaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());

            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var sustanciaDto = new SustanciaDto
            {
                Id = 1,
                Nombre = "Sustancia1",
                Cas = "452125",
                ListaCaq = "Lista1"
            };

            var result = await controller.Post(sustanciaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        //CrearSustancia_DevuelveErrorModeloInvalido
        [Fact]
        public async Task CrearSustancia_DevuelveErrorModeloInvalido()
        {
            var sustanciaDto = new SustanciaDto
            {
                Id = 1,
                Nombre = "",
                Cas = "452125",
                ListaCaq = "Lista1"
            };

            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("Nombre", "El campo Nombre es requerido");

            var result = await controller.Post(sustanciaDto);

            Assert.IsType<ODataErrorResult>(result);
        }

        //EditarSustancia_DevuelveOk
        [Fact]
        public async Task EditarSustancia_DevuelveOk()
        {
            var sustancia = new Sustancia
            {
                Id = 1,
                Nombre = "Sustancia1",
                Cas = "452125",
                ListaCaq = "Lista1"
            };

            mockMediator.Setup(m => m.Send(It.IsAny<EditarSustanciaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(sustancia, sustancia));

            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var sustanciaDto = new SustanciaDto
            {
                Id = 1,
                Nombre = "Sustancia1",
                Cas = "452125",
                ListaCaq = "Lista1"
            };

            var deltaDto = new Delta<SustanciaDto>();
            deltaDto.Patch(sustanciaDto);

            //Act
            var result = await controller.Patch(1, deltaDto);

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        //EditarSustancia_DevuelveError
        [Fact]
        public async Task EditarSustancia_DevuelveError()
        {
            mockMediator.Setup(m => m.Send(It.IsAny<EditarSustanciaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());

            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var sustanciaDto = new SustanciaDto
            {
                Id = 1,
                Nombre = "Sustancia1",
                Cas = "452125",
                ListaCaq = "Lista1"
            };

            var deltaDto = new Delta<SustanciaDto>();
            deltaDto.Patch(sustanciaDto);

            var result = await controller.Patch(1, deltaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        //EditarSustancia_DevuelveErrorModeloInvalido
        [Fact]
        public async Task EditarSustancia_DevuelveErrorModeloInvalido()
        {
            var sustanciaDto = new SustanciaDto
            {
                Id = 1,
                Nombre = "",
                Cas = "452125",
                ListaCaq = "Lista1"
            };

            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("Nombre", "El campo Nombre es requerido");

            var deltaDto = new Delta<SustanciaDto>();
            deltaDto.Patch(sustanciaDto);

            var result = await controller.Patch(1, deltaDto);

            Assert.IsType<ODataErrorResult>(result);
        }

        //EditarSustancia_DevuelveErrorSustanciaNoEncontrado
        [Fact]
        public async Task EditarSustancia_DevuelveErrorSustanciaNoEncontrado()
        {
            mockMediator.Setup(m => m.Send(It.IsAny<EditarSustanciaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());

            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var sustanciaDto = new SustanciaDto
            {
                Id = 1,
                Nombre = "",
                Cas = "452125",
                ListaCaq = "Lista1"
            };

            var deltaDto = new Delta<SustanciaDto>();
            deltaDto.Patch(sustanciaDto);

            var result = await controller.Patch(1, deltaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        //EliminarSustancia_DevuelveOk
        [Fact]
        public async Task EliminarSustancia_DevuelveOk()
        {
            var sustancia = new Sustancia
            {
                Id = 1,
                Nombre = "",
                Cas = "452125",
                ListaCaq = "Lista1"
            };

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarSustanciaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(sustancia);

            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Delete(1);

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        //EliminarSustancia_DevuelveError
        [Fact]
        public async Task EliminarSustancia_DevuelveError()
        {
            mockMediator.Setup(m => m.Send(It.IsAny<EliminarSustanciaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());

            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        //EliminarSustancia_DevuelveErrorModeloInvalido
        [Fact]
        public async Task EliminarSustancia_DevuelveErrorModeloInvalido()
        {
            var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("IdSustancia", "El campo IdSustancia es requerido");

            var result = await controller.Delete(0);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarSustancias_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarSustanciasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new SustanciaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarSustancias_DevuelveError()
        {

            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarSustanciasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new SustanciaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarSustancias_DevuelveErrorModeloInvalido()
        {

            var controller = new SustanciaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            IEnumerable<ImportarSustanciaDto> objetos = new List<ImportarSustanciaDto>
            {
                new ImportarSustanciaDto
                {
                    Nombre = "Nombre1",
                    Cas = "435345",
                    ListaCaq = "lista"
                }
            };

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", objetos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new SustanciaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
                new SustanciaDto { Id = 1, Nombre = "Nombre 1" },
                new SustanciaDto { Id = 2, Nombre = "Nombre 2" }
            };

            IEnumerable<ImportarSustanciaDto> bloques = objetos.Cast<ImportarSustanciaDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", bloques);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new SustanciaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarSustanciaDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new SustanciaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new SustanciaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarSustanciaDto { Nombre = "" }
            };

            IEnumerable<ImportarSustanciaDto> bloques = objetos.Cast<ImportarSustanciaDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", bloques);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new SustanciaController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

    }
}
