using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EliminarCaracteristicaTipoProducto;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.CrearRequisito;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EditarRequisito;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisito;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisitos;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Requisitos.Queries.ObtenerRequisitoPorId;
using VUCE3.Catalogos.Aplicacion.Requisitos.Queries.ObtenerRequisitos;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Tests.Helper;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class RequisitosControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerRequisitos_OK()
        {
            //Arange
            var lstRequisitos = new List<Requisito>()
            {
                new Requisito()
                {
                    Id =1,
                    Codigo = "01",
                    Descripcion = "Descripción 1",
                    Version = "Version 1",
                    IdPais = 1,
                    Activo = true,
                    IdInstitucion = 1
                },
                new Requisito()
                {
                    Id =2,
                    Codigo = "02",
                    Descripcion = "Descripción 2",
                    Version = "Version 1",
                    IdPais = 1,
                    Activo = true,
                    IdInstitucion = 1
                }
            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerRequisitosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstRequisitos);

            var controller = new RequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<RequisitoDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<RequisitoDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Descripción 1", querydto[0].Descripcion);

        }

        [Fact]
        public async Task ObtenerRequisitos_Error()
        {
            var error = ErroresRequisito.RequisitoNoEncontrado;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerRequisitosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new RequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }

        [Fact]
        public async Task ObtenerRequisitoPorId_Ok()
        {
            //Arrange
            var requisito = new Requisito()
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerRequisitoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(requisito);

            var controller = new RequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<RequisitoDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<RequisitoDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Descripción 1", querydto.Descripcion);
        }
        [Fact]
        public async Task ObtenerRequisitoPorId_Error()
        {
            var error = ErroresRequisito.RequisitoNoEncontrado;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerRequisitoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new RequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }
        
        [Fact]
        public async Task ObtenerRequisitoPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();
            var controller = new RequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostRequisito_DevuelveCreated()
        {
            var requisitoDto = new RequisitoDto()
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            var requisito = new Requisito()
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearRequisitoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(requisito);

            var controller = new RequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(requisitoDto);

            var okResult = Assert.IsType<CreatedODataResult<RequisitoDto>>(result);
            var resultRequisito = Assert.IsType<RequisitoDto>(okResult.Value);
            Assert.Equal("Descripción 1", resultRequisito.Descripcion);
        }
        [Fact]
        public async Task PostRequisito_DevuelveBadRequest()
        {
            var requisitoDto = new RequisitoDto()
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            var requisito = new Requisito()
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearRequisitoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(requisito);

            var controller = new RequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(requisitoDto);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PostRequisito_DevuelveProblem()
        {
            var requisitoDto = new RequisitoDto()
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearRequisitoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new RequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(requisitoDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
        [Fact]
        public async Task PatchRequisito_InvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new RequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<RequisitoDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchRequisito_Ok()
        {
            var requisito = new Requisito()
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            var requisitoDto = new RequisitoDto()
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version ="Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            var delta = new Delta<RequisitoDto>(requisitoDto.GetType());
            delta.TrySetPropertyValue(nameof(requisitoDto.Descripcion), "Descripción 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarRequisitoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(requisito, requisito));

            var controller = new RequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchRequisito_NotFound()
        {
            var error = ErroresRequisito.RequisitoNoEncontrado;

            var requisitoDto = new RequisitoDto()
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            var delta = new Delta<RequisitoDto>(requisitoDto.GetType());
            delta.TrySetPropertyValue(nameof(requisitoDto.Descripcion), "Descripción 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarRequisitoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new RequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }

        [Fact]
        public async Task DeleteRequisito_NotFound()
        {
            var error = ErroresRequisito.RequisitoNoEncontrado;
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarRequisitoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new RequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task DeleteRequisito_Ok()
        {
            var requisito = new Requisito()
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarRequisitoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(requisito);

            var controller = new RequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteRequisito_InvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new RequisitosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task BorradoMasivoRequisitoOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarRequisitosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new RequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarRequisitosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new RequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task BorradoMasivoRequisito_DevuelveErrorModeloInvalido()
        {

            var controller = new RequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            var datos = new List<ImportarRequisitoDto>
            {
                 new ImportarRequisitoDto
                {
                    Codigo ="01", Descripcion ="Requisito 1",IdInstitucion =1,Version ="Version 1",Pais="Costa Rica"
                },
                new ImportarRequisitoDto
                {
                    Codigo ="02", Descripcion ="Requisito 2",IdInstitucion =1,Version ="Version 1",Pais="Costa Rica"
                }};
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", datos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new RequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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

            IEnumerable<ImportarRequisitoDto> requisito = objetos.Cast<ImportarRequisitoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", requisito);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new RequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarRequisitoDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new RequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new RequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarRequisitoCommandDto
                {
                    Codigo ="01", Descripcion ="Requisito 1",IdInstitucion =1,Version ="Version 1",Pais="Costa Rica"
                }
            };

            IEnumerable<ImportarRequisitoDto> familias = objetos.Cast<ImportarRequisitoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", familias);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new RequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

    }
}
