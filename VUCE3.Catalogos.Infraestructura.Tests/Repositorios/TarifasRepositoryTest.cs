using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class TarifasRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;

        public TarifasRepositoryTest()
        {
            var dbName = $"BDTestCatalogos_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        }

        [Fact]
        public async Task ObtenerTarifas()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Tarifas.AddRange(
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
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new TarifasRepository(context);
                var result = await repository.ObtenerTarifas();

                Assert.False(result.IsError);
                Assert.NotNull(result.Value);
                Assert.IsType<List<Tarifa>>(result.Value);
            }
        }

        [Fact]
        public async Task ObtenerTarifaPorId()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Tarifas.AddRange(
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
                },
                new Tarifa
                {
                    Id = 4,
                    CodigoTarifa = "Cod 4",
                    DescripcionTarifa = "Tarifa 4",
                    FechaFin = DateTime.Now,
                    FechaInicio = DateTime.Now,
                    IdInstitucion = 4,
                    IdMoneda = 4,
                    IdServicioPlataforma = 4,
                    IdTarifa = 4,
                    IdTipoServicioPlataforma = 4,
                    Importe = 4,
                    ServicioPlataforma = "Servicio 4",
                    TipoServicioPlataforma = "Tipo 4",
                    TipoTarifa = "Tipo 4"
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new TarifasRepository(context);
                var result = await repository.ObtenerTarifaPorId(3);

                Assert.False(result.IsError);
                Assert.NotNull(result.Value);
                Assert.IsType<Tarifa>(result.Value);
            }
        }

        [Fact]
        public async Task ObtenerTarifaPorId_NoEncontrado()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Tarifas.AddRange(
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
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new TarifasRepository(context);
                var result = await repository.ObtenerTarifaPorId(4);

                //Assert
                Assert.True(result.IsError);
                Assert.Null(result.Value);
            }
        }
    }
}
