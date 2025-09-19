using MediatR;
using Moq;
using VUCE3.Catalogos.Dominio.Entidades;
using Microsoft.AspNetCore.Mvc;
using VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivos;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivoPorId;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.CrearCultivo;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EditarCultivo;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EliminarCultivo;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EliminarCultivos;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.ImportarDatos;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Tests.Helper;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class CultivoControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerCultivos_DevuelveOk()
        {
            var cultivos = new List<Cultivo>
            {
                new Cultivo
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Cultivo 1",
                    NombreCientifico = "Cientifico 1",
                    IdVariedad = 2,
                    Variedad = new Variedad { Id = 2,Codigo = "2", Nombre = "Variedad 2" }
                },
                new Cultivo
                {
                    Id = 2,
                   Codigo = "2",
                    Nombre = "Cultivo 2",
                    NombreCientifico = "Cientifico 2",
                    IdVariedad = 1,
                    Variedad = new Variedad { Id = 1, Codigo = "1", Nombre = "Variedad" }
                },
                new Cultivo
                {
                    Id = 3,
                   Codigo = "3",
                    Nombre = "Cultivo 3",
                    NombreCientifico = "Cientifico 3",
                    IdVariedad = 3,
                    Variedad = new Variedad { Id = 3,Codigo = "3", Nombre = "Variedad 3" }
                }
            };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerCultivosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(cultivos);

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Get();

            //Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var cultivosDto = Assert.IsType<List<CultivoDto>>(objectResult.Value);

            Assert.Equal(cultivos.Count, cultivosDto.Count);
            for (int i = 0; i < cultivos.Count; i++)
            {
                Assert.Equal(cultivos[i].Id, cultivosDto[i].Id);
                Assert.Equal(cultivos[i].Codigo, cultivosDto[i].Codigo);
                Assert.Equal(cultivos[i].Nombre, cultivosDto[i].Nombre);
                Assert.Equal(cultivos[i].Variedad.Id, cultivosDto[i].IdVariedad);
            }
        }

        [Fact]
        public async Task ObtenerCultivos_DevuelveError()
        {           

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerCultivosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());           
            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Get();

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ObtenerCultivoPorId_DevuelveOk()
        {
            var cultivo = new Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 2,
                Variedad = new Variedad { Id = 2,Codigo = "2", Nombre = "Variedad 2" }
            };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerCultivoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(cultivo);

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Get(1);

            //Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var cultivoDto = Assert.IsType<CultivoDto>(objectResult.Value);

            Assert.Equal(cultivo.Id, cultivoDto.Id);
            Assert.Equal(cultivo.Codigo, cultivoDto.Codigo);
            Assert.Equal(cultivo.Nombre, cultivoDto.Nombre);
            Assert.Equal(cultivo.NombreCientifico, cultivoDto.NombreCientifico);
            Assert.Equal(cultivo.Variedad.Id, cultivoDto.IdVariedad);
        }

        [Fact]
        public async Task ObtenerCultivoPorId_DevuelveError()
        {            

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerCultivoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());           

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Get(1);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ObtenerCultivoPorId_DevuelveErrorModeloInvalido()
        {            

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("IdVariedad", "El campo IdVariedad es requerido");

            var result = await controller.Get(0);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task CrearCultivo_DevuelveOk()
        {
            var cultivo = new Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                Variedad = new Variedad { Id = 2,Codigo = "2", Nombre = "Variedad 2" }
            };

            mockMediator.Setup(m => m.Send(It.IsAny<CrearCultivoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(cultivo);

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var cultivoDto = new CultivoDto
            {
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 2
            };

            //Act
            var result = await controller.Post(cultivoDto);

            //Assert
            var objectResult = Assert.IsType<CreatedODataResult<CultivoDto>>(result);
        }

        [Fact]
        public async Task CrearCultivo_DevuelveError()
        {          

            mockMediator.Setup(m => m.Send(It.IsAny<CrearCultivoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());           

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var cultivoDto = new CultivoDto
            {
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 2
            };

            var result = await controller.Post(cultivoDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task CrearCultivo_DevuelveErrorModeloInvalido()
        {
            var cultivo = new CultivoDto
            {

                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1
            };            

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("IdVariedad", "El campo IdVariedad es requerido");

            var result = await controller.Post(cultivo);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveOk()
        {
            var cultivo = new Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                Variedad = new Variedad { Id = 2,Codigo = "2", Nombre = "Variedad 2" }
            };

            mockMediator.Setup(m => m.Send(It.IsAny<EditarCultivoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(cultivo, cultivo));

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var cultivoDto = new CultivoDto
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 2
            };

            var deltaDto = new Delta<CultivoDto>();
            deltaDto.Patch(cultivoDto);

            //Act
            var result = await controller.Patch(1, deltaDto);

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveError()
        {           

            mockMediator.Setup(m => m.Send(It.IsAny<EditarCultivoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());            

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var cultivoDto = new CultivoDto
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 2
            };

            var deltaDto = new Delta<CultivoDto>();
            deltaDto.Patch(cultivoDto);

            var result = await controller.Patch(1, deltaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveErrorModeloInvalido()
        {
            var cultivoDto = new CultivoDto
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1
            };            

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("IdVariedad", "El campo IdVariedad es requerido");

            var deltaDto = new Delta<CultivoDto>();
            deltaDto.Patch(cultivoDto);

            var result = await controller.Patch(1, deltaDto);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveErrorCultivoNoEncontrado()
        {           

            mockMediator.Setup(m => m.Send(It.IsAny<EditarCultivoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());            

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var cultivoDto = new CultivoDto
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 2
            };

            var deltaDto = new Delta<CultivoDto>();
            deltaDto.Patch(cultivoDto);

            var result = await controller.Patch(1, deltaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveErrorVariedadNoEncontrada()
        {            

            mockMediator.Setup(m => m.Send(It.IsAny<EditarCultivoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());            

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var cultivoDto = new CultivoDto
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 2
            };

            var deltaDto = new Delta<CultivoDto>();
            deltaDto.Patch(cultivoDto);

            var result = await controller.Patch(1, deltaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarCultivo_DevuelveOk()
        {
            var cultivo = new Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                Variedad = new Variedad { Id = 2,Codigo = "2", Nombre = "Variedad 2" }
            };

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCultivoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(cultivo);

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Delete(1);

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task EliminarCultivo_DevuelveError()
        {         

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCultivoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());         

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarCultivo_DevuelveErrorModeloInvalido()
        {
            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("IdCultivo", "El campo IdCultivo es requerido");

            var result = await controller.Delete(0);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            var param = new ODataActionParameters();
            var datosCultivos = new List<ImportarCultivoDto>
            {
                new ImportarCultivoDto
                {
                    Codigo = "1",
                    Nombre = "Cultivo 1",
                    NombreCientifico = "Cientifico 1",
                    CodigoVariedad = "1"
                },
                new ImportarCultivoDto
                {
                   Codigo = "2",
                    Nombre = "Cultivo 2",
                    NombreCientifico = "Cientifico 2",
                    CodigoVariedad = "2"
                }
            };

            param.Add("modo", 1);
            param.Add("datos", datosCultivos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

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
                new PaisDto { Id = 1, Nombre = "Nombre 1", CodigoA2 = "N1",CodigoNumerico="123", CodigoC3 = "A" },
                new PaisDto { Id = 2, Nombre = "Nombre 2", CodigoA2 = "N2", CodigoNumerico = "456", CodigoC3 ="B" }
            };

            IEnumerable<ImportarCultivoDto> cultivos = objetos.Cast<ImportarCultivoDto>();
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", cultivos);           

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);           

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveError()
        {
            //Arrange            

            var datosCultivos = new List<ImportarCultivoDto>
            {
                new ImportarCultivoDto
                {
                    Codigo = "1",
                    Nombre = "Cultivo 1",
                    NombreCientifico = "Cientifico 1",
                    CodigoVariedad = "1"
                },
                new ImportarCultivoDto
                {
                   Codigo = "2",
                    Nombre = "Cultivo 2",
                    NombreCientifico = "Cientifico 2",
                    CodigoVariedad = "2"
                }
            };

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", datosCultivos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());            

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

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

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("Modo", "El campo Modo es requerido");

            //Act
            var result = await controller.ImportarDatos(new ODataActionParameters());

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarCultivos_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCultivosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarCultivos_DevuelveError()
        {           

            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarCultivosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());        

            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarCultivos_DevuelveErrorModeloInvalido()
        {
            var controller = new CultivoController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_FicheroDatosNullError()
        {
            //Arrange
            IEnumerable<object> objetos = new List<object>
            {
                new ImportarCultivoDto { Codigo = "123" }
            };

            IEnumerable<ImportarCultivoDto> cultivos = objetos.Cast<ImportarCultivoDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", cultivos);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new CultivoController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

    }
}
