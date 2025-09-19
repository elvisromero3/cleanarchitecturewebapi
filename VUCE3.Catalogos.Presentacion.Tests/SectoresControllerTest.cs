using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.CrearSector;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.EditarSector;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.EliminarSector;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.EliminarSectores;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Sectores.Queries.ObtenerSectores;
using VUCE3.Catalogos.Aplicacion.Sectores.Queries.ObtenerSectorPorId;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Tests.Helper;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class SectoresControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerSectores_OK()
        {
            //Arange
            var lstSectores = new List<Sector>()
            {
                new Sector()
                {
                    Id =1,
                    Nombre = "Sector 1",
                     Codigo = "Codigo 1"

                },
                new Sector()
                {
                    Id =2,
                    Nombre = "Sector 2",
                     Codigo = "Codigo 2"
                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerSectoresQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstSectores);

            var controller = new SectoresController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<SectorDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<SectorDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Sector 1", querydto[0].Nombre);
        }

        [Fact]
        public async Task ObtenerSectorPorId_Ok()
        {
            //Arrange
            var sector = new Sector()
            {
                Id = 1,
                Nombre = "Sector 1",
                Codigo = "Codigo 1"

            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerSectorPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(sector);

            var controller = new SectoresController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<SectorDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SectorDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Sector 1", querydto.Nombre);
        }

        [Fact]
        public async Task ObtenerSectorPorId_Error()
        {
            var error = ErroresCasa.NoEncontrada;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerSectorPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new SectoresController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerSector_Error()
        {
            var error = ErroresSector.SectorNoEncontrado;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerSectoresQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new SectoresController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerSectorPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();
            var controller = new SectoresController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostSector_DevuelveCreated()
        {
            var sectorDto = new SectorDto()
            {
                Id = 1,
                Nombre = "Sector 1",
                Codigo = "Codigo 1"

            };

            var sector = new Sector()
            {
                Id = 1,
                Nombre = "Sector 1",
                Codigo = "Codigo 1"
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearSectorCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(sector);

            var controller = new SectoresController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(sectorDto);

            var okResult = Assert.IsType<CreatedODataResult<SectorDto>>(result);
            var resultSector = Assert.IsType<SectorDto>(okResult.Value);
            Assert.Equal("Sector 1", resultSector.Nombre);
        }

        [Fact]
        public async Task PostSector_DevuelveBadRequest()
        {
            var sectorDto = new SectorDto()
            {
                Id = 1,
                Nombre = "Sector 1",
                Codigo = "Codigo 1"

            };

            var sector = new Sector()
            {
                Id = 1,
                Nombre = "Sector 1",
                Codigo = "Codigo 1"
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearSectorCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(sector);

            var controller = new SectoresController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(sectorDto);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostSector_DevuelveProblem()
        {
            var sectorDto = new SectorDto()
            {
                Id = 1,
                Nombre = "Sector 1",
                Codigo = "Codigo 1"

            };

            var error = Error.Failure();

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearSectorCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);


            var controller = new SectoresController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(sectorDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task PatchSector_InvalidModel()
        {

            var mockSender = new Mock<ISender>();

            var controller = new SectoresController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<SectorDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchSector_Ok()
        {
            var sector = new Sector()
            {
                Id = 1,
                Nombre = "Sector 1",
                Codigo = "Codigo 1"
            };

            var sectorDto = new SectorDto()
            {
                Id = 1,
                Nombre = "Sector 1",
                Codigo = "Codigo 1"
            };

            var delta = new Delta<SectorDto>(sectorDto.GetType());
            delta.TrySetPropertyValue(nameof(sectorDto.Nombre), "Sector 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarSectorCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(sector, sector));

            var controller = new SectoresController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchSector_NotFound()
        {
            var error = ErroresSector.SectorNoEncontrado;

            var sectorDto = new SectorDto()
            {
                Id = 1,
                Nombre = "Sector 1",
                Codigo = "Codigo 1"
            };

            var delta = new Delta<SectorDto>(sectorDto.GetType());
            delta.TrySetPropertyValue(nameof(sectorDto.Nombre), "Sector 1");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarSectorCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new SectoresController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task DeleteSector_NotFound()
        {
            var error = ErroresSector.SectorNoEncontrado;
            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarSectorCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new SectoresController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
        }

        [Fact]
        public async Task DeleteSector_Ok()
        {
            var grupo = new Sector()
            {
                Id = 1,
                Nombre = "Sector 1",
                Codigo = "Codigo 1"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarSectorCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(grupo);

            var controller = new SectoresController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteSector_InvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new SectoresController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            var param = new ODataActionParameters();
            var datosSectores = new List<ImportarSectorDto>
            {
                new ImportarSectorDto
                {
                    Nombre = "Sector uno",
                    Codigo = "Codigo 1"
                },
                new ImportarSectorDto
                {
                    Nombre = "Sector dos",
                    Codigo = "Codigo 1"
                }
            };

            param.Add("modo", 1);
            param.Add("datos", datosSectores);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new SectoresController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_FicheroEstructuraError()
        {             //Arrange
            var param = new ODataActionParameters();
            var datosSectores = new List<ImportarSectorDto>
            {
                new ImportarSectorDto
                {
                    Nombre = "Sector uno",
                    Codigo = "Codigo 1"
                },
                new ImportarSectorDto
                {
                    Nombre = "Sector dos",
                    Codigo = "Codigo 2"
                }
            };
            param.Add("modo", 1);
            param.Add("datos", datosSectores);
            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(ErroresSector.DatosDuplicadosArchivo);
            var controller = new SectoresController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_ErrorCamposNullosOVacios()
        {             //Arrange
            var param = new ODataActionParameters();
            var datosSectores = new List<ImportarSectorDto>
            {
                new ImportarSectorDto
                {
                    Nombre = "",
                    Codigo = "Codigo 1"
                },
                new ImportarSectorDto
                {
                    Nombre = "",
                    Codigo = "Codigo 1"
                }
            };
            param.Add("modo", 1);
            param.Add("datos", datosSectores);
            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(ErroresSector.DatosDuplicadosArchivo);
            var controller = new SectoresController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_ErrorDatosDuplicados()
        {
            var param = new ODataActionParameters();
            var datosSectores = new List<ImportarSectorDto>
            {
                new ImportarSectorDto
                {
                    Nombre = "Sector uno",
                    Codigo = "Codigo 1"
                },
                new ImportarSectorDto
                {
                    Nombre = "Sector dos",
                    Codigo = "Codigo 1"
                }
            };
            param.Add("modo", 1);
            param.Add("datos", datosSectores);
            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(ErroresSector.DatosDuplicados);
            var controller = new SectoresController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveErrorModeloInvalido()
        {
            //Arrange
            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(ErroresSector.DatosDuplicados);
            var controller = new SectoresController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("Modo", "El campo Modo es requerido");

            //Act
            var result = await controller.ImportarDatos(new ODataActionParameters());

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task BorradoMasivo_DevuelveOk()
        {
            //Arrange
            var param = new ODataActionParameters();
            var items = new List<int> { 1, 2, 3 };
            param.Add("items", items);
            mockMediator.Setup(m => m.Send(It.IsAny<EliminarSectoresCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);
            var controller = new SectoresController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            //Act
            var result = await controller.BorradoMasivo(param);
            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task BorradoMasivo_DevuelveError()
        {
            var param = new ODataActionParameters();
            var items = new List<int> { 1, 2, 3 };
            param.Add("items", items);
            mockMediator.Setup(m => m.Send(It.IsAny<EliminarSectoresCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(ErroresSector.SectorNoEncontrado);
            var controller = new SectoresController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            //Act
            var result = await controller.BorradoMasivo(param);
            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task BorradoMasivo_DevuelveErrorModeloInvalido()
        {
            //Arrange
            var controller = new SectoresController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");
            //Act
            var result = await controller.BorradoMasivo(new ODataActionParameters());
            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }
    }
}
