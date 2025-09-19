using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivoPorId;
using VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivos;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifaPorId;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifas;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifasPorId;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class TarifasTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();

        [Fact]
        public async Task ObtenerTarifas_DevuelveOk()
        {
            // Arrange
            var tarifas = new List<Dominio.Entidades.Tarifa>
            {
                new Dominio.Entidades.Tarifa
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
                    TipoTarifa = "Tipo 1",
                    Moneda = new Dominio.Entidades.Moneda
                    {
                        Id = 1,
                        Nombre = "Moneda 1"
                    }
                },
                new Dominio.Entidades.Tarifa
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
                new Dominio.Entidades.Tarifa
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

            mockRepo.Setup(repo => repo.TarifasRepository.ObtenerTarifas()).ReturnsAsync(tarifas);

            var handler = new ObtenerTarifasQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ObtenerTarifasQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(result.Value.Count, result.Value?.Count);
        }

        [Fact]
        public async Task ObtenerTarifaPorId_DevuelveOk()
        {
            // Arrange
            var tarifas = new Dominio.Entidades.Tarifa
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

            mockRepo.Setup(repo => repo.TarifasRepository.ObtenerTarifaPorId(1)).ReturnsAsync(tarifas);

            var handler = new ObtenerTarifaPorIdQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ObtenerTarifaPorIdQuery { IdTarifa = 1 }, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(tarifas.Id, result.Value.Id);
        }

        [Fact]
        public async Task ObtenerCultivoPorId_DevuelveError()
        {
            // Arrange
            var erroror = ErroresTarifa.NoEncontrada;

            mockRepo.Setup(repo => repo.TarifasRepository.ObtenerTarifaPorId(1)).ReturnsAsync(erroror);

            var handler = new ObtenerTarifaPorIdQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ObtenerTarifaPorIdQuery { IdTarifa = 1 }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }
    }
}
