using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifas;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifasPorId;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class TarifasControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerTarifas_DevuelveOk()
        {
            var tarifas = new List<Tarifa>
            {
                new Tarifa
                {
                    Id = 1,
                    CodigoTarifa = "Cod 1",
                    DescripcionTarifa = "Tarifa 1",
                    FechaFin = DateTime.Now,
                    FechaInicio = DateTime.Now,
                    IdInstitucion = 1,
                    IdMoneda = 1,
                    IdServicioPlataforma = 1,
                    IdTarifa = 1,
                    IdTipoServicioPlataforma = 1,
                    Importe = 1,
                    ServicioPlataforma = "Servicio 1",
                    TipoServicioPlataforma = "Tipo 1",
                    TipoTarifa = "Tipo 1"
                },
                new Tarifa
                {
                    Id = 2,
                    CodigoTarifa = "Cod 2",
                    DescripcionTarifa = "Tarifa 2",
                    FechaFin = DateTime.Now,
                    FechaInicio = DateTime.Now,
                    IdInstitucion = 2,
                    IdMoneda = 2,
                    IdServicioPlataforma = 2,
                    IdTarifa = 2,
                    IdTipoServicioPlataforma = 2,
                    Importe = 2,
                    ServicioPlataforma = "Servicio 2",
                    TipoServicioPlataforma = "Tipo 2",
                    TipoTarifa = "Tipo 2"
                },
                new Tarifa
                {
                    Id = 3,
                    CodigoTarifa = "Cod 3",
                    DescripcionTarifa = "Tarifa 3",
                    FechaFin = DateTime.Now,
                    FechaInicio = DateTime.Now,
                    IdInstitucion = 3,
                    IdMoneda = 3,
                    IdServicioPlataforma = 3,
                    IdTarifa = 3,
                    IdTipoServicioPlataforma = 3,
                    Importe = 3,
                    ServicioPlataforma = "Servicio 3",
                    TipoServicioPlataforma = "Tipo 3",
                    TipoTarifa = "Tipo 3"
                }
            };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerTarifasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(tarifas);

            var controller = new TarifasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Get();

            //Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var tarifasDto = Assert.IsType<List<TarifaDto>>(objectResult.Value);

            Assert.Equal(tarifas.Count, tarifasDto.Count);
            for (int i = 0; i < tarifas.Count; i++)
            {
                Assert.Equal(tarifas[i].Id, tarifasDto[i].Id);
                Assert.Equal(tarifas[i].DescripcionTarifa, tarifasDto[i].DescripcionTarifa);
            }
        }

        [Fact]
        public async Task ObtenerTarifas_DevuelveError()
        {

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerTarifasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());
            var controller = new TarifasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Get();

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ObtenerTarifaPorId_DevuelveOk()
        {
            var tarifa = new Tarifa
            {
                Id = 1,
                CodigoTarifa = "Cod 1",
                DescripcionTarifa = "Tarifa 1",
                FechaFin = DateTime.Now,
                FechaInicio = DateTime.Now,
                IdInstitucion = 1,
                IdMoneda = 1,
                IdServicioPlataforma = 1,
                IdTarifa = 1,
                IdTipoServicioPlataforma = 1,
                Importe = 1,
                ServicioPlataforma = "Servicio 1",
                TipoServicioPlataforma = "Tipo 1",
                TipoTarifa = "Tipo 1"
            };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerTarifaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(tarifa);

            var controller = new TarifasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Get(1);

            //Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var tarifaDto = Assert.IsType<TarifaDto>(objectResult.Value);

            Assert.Equal(tarifa.Id, tarifaDto.Id);
            Assert.Equal(tarifa.DescripcionTarifa, tarifaDto.DescripcionTarifa);
        }

        [Fact]
        public async Task ObtenerTarifaPorId_DevuelveError()
        {

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerTarifaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());

            var controller = new TarifasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Get(1);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ObtenerTarifaPorId_InvalidModel()
        {

            var controller = new TarifasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);

            Assert.IsType<ODataErrorResult>(result);
        }
    }
}
