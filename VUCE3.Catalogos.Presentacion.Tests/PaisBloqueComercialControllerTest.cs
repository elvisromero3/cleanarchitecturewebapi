using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Queries.ObtenerPaisBloqueComercial;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Queries.ObtenerPaisBloqueComercialPorId;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.CrearPaisBloqueComercial;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EditarPaisBloqueComercial;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EliminarPaisBloqueComercial;

using VUCE3.Catalogos.Dominio.Errores;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using AutoMapper;
using VUCE3.Catalogos.Presentacion.Validadores;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EliminarPaisBloquesComerciales;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.Importardatos;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisitos;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class PaisBloqueComercialsControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerPaisBloqueComercials_OK()
        {
            //Arange
            var lstPaisBloqueComercials = new List<PaisBloqueComercial>()
            {
                new PaisBloqueComercial()
                {
                    Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1

                },
                new PaisBloqueComercial()
                {
                   Id = 2,
                IdBloqueComercial = 1,
                IdPais = 1

                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerPaisBloqueComercialQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstPaisBloqueComercials);

            var controller = new PaisBloqueComercialController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<PaisBloqueComercialDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<PaisBloqueComercialDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal(1, querydto[0].IdBloqueComercial);

        }
        [Fact]
        public async Task ObtenerPaisBloqueComercialPorId_Ok()
        {
            //Arrange
            var PaisBloqueComercial = new PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1

            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerPaisBloqueComercialPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(PaisBloqueComercial);

            var controller = new PaisBloqueComercialController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<PaisBloqueComercialDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<PaisBloqueComercialDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal(1, querydto.IdBloqueComercial);
        }
        [Fact]
        public async Task ObtenerPaisBloqueComercialPorId_Error()
        {
            var error = ErroresPaisBloqueComercial.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerPaisBloqueComercialPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new PaisBloqueComercialController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerPaisBloqueComercial_Error()
        {
            var error = ErroresPaisBloqueComercial.NoEncontrada;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerPaisBloqueComercialQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new PaisBloqueComercialController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }
        [Fact]
        public async Task ObtenerPaisBloqueComercialPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();            
            var controller = new PaisBloqueComercialController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostPaisBloqueComercial_DevuelveCreated()
        {
            var paisBloqueComercialDto = new PaisBloqueComercialDto()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1

            };

            var paisBloqueComercial = new PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };



            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearPaisBloqueComercialCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(paisBloqueComercial);

            var controller = new PaisBloqueComercialController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(paisBloqueComercialDto);

            var okResult = Assert.IsType<CreatedODataResult<PaisBloqueComercialDto>>(result);
            var resultPaisBloqueComercial = Assert.IsType<PaisBloqueComercialDto>(okResult.Value);
            Assert.Equal(1, resultPaisBloqueComercial.IdBloqueComercial);
        }
        [Fact]
        public async Task PostPaisBloqueComercial_DevuelveBadRequest()
        {
            var paisBloqueComercialDto = new PaisBloqueComercialDto()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            var paisBloqueComercial = new PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearPaisBloqueComercialCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(paisBloqueComercial);            

            var controller = new PaisBloqueComercialController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(paisBloqueComercialDto);

            Assert.IsType<ODataErrorResult>(result);
        }
        [Fact]
        public async Task PostPaisBloqueComercial_DevuelveProblem()
        {
            var paisBloqueComercialDto = new PaisBloqueComercialDto()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            var error = Error.Failure();            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearPaisBloqueComercialCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);
            

            var controller = new PaisBloqueComercialController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(paisBloqueComercialDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }
        [Fact]
        public async Task PatchPaisBloqueComercial_InvalidModel()
        {

            var mockSender = new Mock<ISender>();            

            var controller = new PaisBloqueComercialController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<PaisBloqueComercialDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchPaisBloqueComercial_Ok()
        {
            var paisBloqueComercial = new PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            var PaisBloqueComercialDto = new PaisBloqueComercialDto()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            var delta = new Delta<PaisBloqueComercialDto>(PaisBloqueComercialDto.GetType());
            delta.TrySetPropertyValue(nameof(PaisBloqueComercialDto.IdBloqueComercial), 1);

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarPaisBloqueComercialCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(paisBloqueComercial, paisBloqueComercial));

            var controller = new PaisBloqueComercialController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchPaisBloqueComercial_NotFound()
        {            
            var error = ErroresPaisBloqueComercial.NoEncontrada;

            var paisDto = new PaisBloqueComercialDto()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            var delta = new Delta<PaisBloqueComercialDto>(paisDto.GetType());
            delta.TrySetPropertyValue(nameof(paisDto.IdBloqueComercial), 1);

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarPaisBloqueComercialCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new PaisBloqueComercialController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task DeletePaisBloqueComercial_NotFound()
        {
            var error = ErroresPaisBloqueComercial.NoEncontrada;         
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarPaisBloqueComercialCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);         

            var controller = new PaisBloqueComercialController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeletePaisBloqueComercial_Ok()
        {
            var grupo = new PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarPaisBloqueComercialCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(grupo);

            var controller = new PaisBloqueComercialController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeletePaisBloqueComercial_InvalidModel()
        {           

            var mockSender = new Mock<ISender>();            
            
            var controller = new PaisBloqueComercialController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task BorradoMasivoPaisBloqueComercialOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            
            mockMediator.Setup(m => m.Send(It.IsAny<EliminarPaisBloquesComercialesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new PaisBloqueComercialController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        //[Fact]
        //public async Task BorradoMasivoPaisBloqueComercial_DevuelveError()
        //{
        //    var param = new ODataActionParameters();
        //    param.Add("items", new List<int> { 1, 2, 3 });

        //    mockMediator.Setup(m => m.Send(It.IsAny<EliminarPaisBloquesComercialesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

        //    var controller = new RequisitosController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

        //    var result = await controller.BorradoMasivo(param);

        //    //Assert
        //    var objectResult = Assert.IsType<ODataErrorResult>(result);
        //    var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
        //    Assert.Equal("500", problemDetails.ErrorCode);
        //}

        [Fact]
        public async Task BorradoMasivoPaisBloqueComercial_DevuelveErrorModeloInvalido()
        {

            var controller = new PaisBloqueComercialController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            var datos = new List<ImportarPaisBloqueComercialDto>
            {
                 new ImportarPaisBloqueComercialDto
                {
                    BloqueComercial ="bloque 1",
                    Pais="Costa Rica"
                },
                new ImportarPaisBloqueComercialDto
                {
                    BloqueComercial = "bloque 2",
                    Pais="Costa Rica"
                }};
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", datos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommandHandler>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new PaisBloqueComercialController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
                new { IdBloqueComercial =1,
                        Pais="Costa Rica" },
                new { IdBloqueComercial =2,
                        Pais="Costa Rica" }
            };

            IEnumerable<ImportarPaisBloqueComercialDto> PaisBloqueComercial = objetos.Cast<ImportarPaisBloqueComercialDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", PaisBloqueComercial);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new PaisBloqueComercialController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarPaisBloqueComercialDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());

            var controller = new PaisBloqueComercialController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new PaisBloqueComercialController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
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
                new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial ="bloque 1",
                    Pais="Costa Rica"
                }
            };

            IEnumerable<ImportarPaisBloqueComercialDto> paisBloqueComercial = objetos.Cast<ImportarPaisBloqueComercialDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", paisBloqueComercial);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new PaisBloqueComercialController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }


    }
}
