using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Queries.ObtenerSustanciaControladas;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Queries.ObtenerSustanciaControladaPorId;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.CrearSustanciaControlada;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.EditarSustanciaControlada;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.EliminarSustanciaControlada;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.EliminarSustanciasControladas;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.ImportarDatos;
using VUCE3.Catalogos.Dominio.Errores;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.EliminarFamilias;


namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class SustanciaControladasControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerSustanciaControladas_OK()
        {
            //Arange
            var lstSustanciaControladas = new List<SustanciaControlada>()
            {
                new SustanciaControlada()
                {
                    Id =1,
                    ClasificacionArancelaria = "Clasificacion 1",
                    ClasificacionAshrae =  "D650",
                    PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                    IdFamilia =1

                },
                new SustanciaControlada()
                {
                    Id =2,
                    ClasificacionArancelaria = "Clasificacion 2",
                    ClasificacionAshrae =  "D650",
                    PotencialCalentamientoGlobal = "Calentamiento Nivel 2",
                     IdFamilia =1
                }
             };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerSustanciaControladasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstSustanciaControladas);

            var controller = new SustanciasControladasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            // Act
            var result = await controller.Get();

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var sustanciascontroladaDto = Assert.IsType<List<SustanciaControladaDto>>(objectResult.Value);

            Assert.Equal(lstSustanciaControladas.Count, sustanciascontroladaDto.Count);
            for (int i = 0; i < lstSustanciaControladas.Count; i++)
            {
                Assert.Equal(lstSustanciaControladas[i].Id, sustanciascontroladaDto[i].Id);
                Assert.Equal(lstSustanciaControladas[i].ClasificacionArancelaria, sustanciascontroladaDto[i].ClasificacionArancelaria);
            }


        }

        //ObtenerSustanciaPorId_DevuelveOk
        [Fact]
        public async Task ObtenerSustanciaControladasPorId_Ok()
        {
            //Arrange
            var sustanciacontrolada = new SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 2",
                IdFamilia =1

            };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerSustanciaControladaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(sustanciacontrolada);


            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerSustanciaControladaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(sustanciacontrolada);

            var controller = new SustanciasControladasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<SustanciaControladaDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SustanciaControladaDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Clasificacion 1", querydto.ClasificacionArancelaria);
        }

        //ObtenerSustanciaPorId_DevuelveError
        [Fact]
        public async Task ObtenerSustanciaControladaPorId_Error()
        {
            var error = ErroresSustanciaControlada.NoEncontrada;


            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerSustanciaControladaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new SustanciasControladasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }
        //ObtenerSustanciaPorId_DevuelveErrorModeloInvalido
        [Fact]
        public async Task ObtenerSustanciaPorId_DevuelveErrorModeloInvalido()
        {
            var controller = new SustanciasControladasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("IdVariedad", "El campo IdVariedad es requerido");

            var result = await controller.Get(0);

            Assert.IsType<ODataErrorResult>(result);
        }

        //CrearSustancia_DevuelveOk
        [Fact]
        public async Task ObtenerSustanciaControlada_Error()
        {
            //
            //Arange
            var lstSustanciaControladas = new List<SustanciaControlada>()
            {
                new SustanciaControlada()
                {
                    Id =1,
                    ClasificacionArancelaria = "Clasificacion 1",
                    ClasificacionAshrae =  "D650",
                    PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                    IdFamilia =1

                },
                new SustanciaControlada()
                {
                    Id =2,
                    ClasificacionArancelaria = "Clasificacion 2",
                    ClasificacionAshrae =  "D650",
                    PotencialCalentamientoGlobal = "Calentamiento Nivel 2",
                     IdFamilia =1
                }
             };

            var error = ErroresSustanciaControlada.NoEncontrada;

            mockMediator.Setup(m => m.Send(It.IsAny<CrearSustanciaControladaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstSustanciaControladas[0]);


            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerSustanciaControladasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new SustanciasControladasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

            //var controller = new SustanciaController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //var sustanciaDto = new SustanciaDto
            //{
            //    Id = 1,
            //    Nombre = "Sustancia1",
            //    Cas = "452125",
            //    ListaCaq = "Lista1"
            //};

            //var result = await controller.Post(sustanciaDto);

            //var objectResult = Assert.IsType<ODataErrorResult>(result);
            //var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            //Assert.Equal("404", problemDetails.ErrorCode);
        }

        //CrearSustancia_DevuelveErrorModeloInvalido
        [Fact]
        public async Task ObtenerSustanciaControladaPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();
            var controller = new SustanciasControladasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        //EditarSustancia_DevuelveOk
        [Fact]
        public async Task PostSustanciaControlada_DevuelveCreated()
        {
            var sustanciacontroladaDto = new SustanciaControladaDto()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 2",
                IdFamilia =1

            };

            var sustanciacontrolada = new SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 2",
                IdFamilia =1
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearSustanciaControladaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(sustanciacontrolada);

            var controller = new SustanciasControladasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(sustanciacontroladaDto);

            var okResult = Assert.IsType<CreatedODataResult<SustanciaControladaDto>>(result);
            var resultSustanciaControlada = Assert.IsType<SustanciaControladaDto>(okResult.Value);
            Assert.Equal("Clasificacion 1", resultSustanciaControlada.ClasificacionArancelaria);
        }
        [Fact]
        public async Task PostSustanciaControlada_DevuelveBadRequest()
        {
            var sustanciacontroladaDto = new SustanciaControladaDto()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 2",
                IdFamilia =1

            };

            var sustanciacontrolada = new SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 2",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 2",
                IdFamilia =1
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearSustanciaControladaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(sustanciacontrolada);

            var controller = new SustanciasControladasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(sustanciacontroladaDto);

            Assert.IsType<ODataErrorResult>(result);
        }


        [Fact]
        public async Task EliminarSustancia_DevuelveOk()
        {
            var sustanciacontroladaDto = new SustanciaControladaDto()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 2",
                IdFamilia =1
            };

            var error = Error.Failure();

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearSustanciaControladaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarSustanciaControladaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new SustanciasControladasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(sustanciacontroladaDto);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task PatchSustanciaControlada_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new SustanciasControladasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<SustanciaControladaDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchSustanciaControlada_Ok()
        {
            var sustanciacontrolada = new SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 2",
                IdFamilia =1
            };

            var sustanciacontroladaDto = new SustanciaControladaDto()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 2",
                IdFamilia =1
            };

            var delta = new Delta<SustanciaControladaDto>(sustanciacontroladaDto.GetType());
            delta.TrySetPropertyValue(nameof(sustanciacontroladaDto.ClasificacionArancelaria), "Clasificacion 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarSustanciaControladaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(sustanciacontrolada, sustanciacontrolada));

            var controller = new SustanciasControladasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchSustanciaControlada_NotFound()
        {
            var error = ErroresSustanciaControlada.NoEncontrada;

            var sustanciacontroladaDto = new SustanciaControladaDto()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 2",
                IdFamilia =1
            };

            var delta = new Delta<SustanciaControladaDto>(sustanciacontroladaDto.GetType());
            delta.TrySetPropertyValue(nameof(sustanciacontroladaDto.ClasificacionArancelaria), "SustanciaControlada 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarSustanciaControladaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new SustanciasControladasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task DeleteSustanciaControlada_NotFound()
        {
            var error = ErroresSustanciaControlada.NoEncontrada;
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarSustanciaControladaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new SustanciasControladasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task DeleteSustanciaControlada_Ok()
        {
            var sustanciacontrolada = new SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 2",
                IdFamilia =1
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarSustanciaControladaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(sustanciacontrolada);

            var controller = new SustanciasControladasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task EliminarSustancia_DevuelveErrorModeloInvalido()
        {
            var controller = new SustanciasControladasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("IdSustancia", "El campo IdSustancia es requerido");

            var result = await controller.Delete(0);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task SustanciaControladas_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarSustanciasControladasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new SustanciasControladasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task SustanciaControladas_DevuelveError()
        {

            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarSustanciasControladasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new SustanciasControladasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task SustanciaControladas_DevuelveErrorModeloInvalido()
        {

            var controller = new SustanciasControladasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
            param.Add("datos", new List<ImportarSustanciaControladaDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new SustanciasControladasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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

            IEnumerable<ImportarSustanciaControladaDto> familias = objetos.Cast<ImportarSustanciaControladaDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", familias);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new SustanciasControladasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarSustanciaControladaDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new SustanciasControladasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new SustanciasControladasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarSustanciaControladaDto {ClasificacionArancelaria="ClasificacionArancelaria" }
            };

            IEnumerable<ImportarSustanciaControladaDto> familias = objetos.Cast<ImportarSustanciaControladaDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", familias);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new SustanciasControladasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }
    } 
}
