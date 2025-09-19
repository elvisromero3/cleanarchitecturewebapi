using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class SustanciaControladaRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public SustanciaControladaRepositoryTest()
        {
            var dbName = $"BDTestDCasas_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<SustanciaControladaRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new SustanciaControladaRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            SustanciaControlada sustanciacontrolada = new()
            {
                Id =1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                Familia = familia,
                TipoGas = "GAS1"
            };

            context.SustanciaControlada.Add(sustanciacontrolada);
            context.Familias.Add(familia);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerSustanciaControladas_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var sustanciacontroladas = repository.ObtenerSustanciaControladas();
            var result = Assert.IsType<List<SustanciaControlada>>(sustanciacontroladas?.Result.Value);

            //Assert
            Assert.Equal("Clasificacion 1", result[0].ClasificacionArancelaria);
        }
        [Fact]
        public async Task ObtenerSustanciaControladasPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var sustanciacontroladas = repository.ObtenerSustanciaControladaPorId(1);
            var result = sustanciacontroladas?.Result.Value;

            //Assert            
            Assert.Equal("Clasificacion 1", result?.ClasificacionArancelaria);
        }
        [Fact]
        public async Task ObtenerSustanciaControladaPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var sustanciacontrolada = repository.ObtenerSustanciaControladaPorId(10);
            var result = sustanciacontrolada?.Result.Errors;

            //Assert            
            Assert.True(sustanciacontrolada?.Result.IsError);
            Assert.Equal("SustanciaControlada.NoEncontrada", result?[0].Code);
            Assert.Equal("Sustancia controlada no encontrada", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarSustanciaControlada_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var sustanciacontrolada = await repository.ObtenerSustanciaControladaPorId(id);
            sustanciacontrolada.Value.ClasificacionArancelaria = "Clasificacion 1";
            sustanciacontrolada.Value.ClasificacionAshrae = "02";

            //Act
            var sustanciacontroladaActualizada = await repository.ActualizarSustanciaControlada(1, listaCambios, sustanciacontrolada.Value);

            //Assert
            Assert.Equal("Clasificacion 1", sustanciacontroladaActualizada.Value.ClasificacionArancelaria);
        }


        [Fact]
        public async Task ValidarSustanciaControlada_True_SiYaExiste()
        {
            // Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            var sustanciaExistente = new SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae = "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                IdFamilia = 1,
                TipoGas = "GAS1"
            };

            var sustanciaAValidar = new SustanciaControlada()
            {
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae = "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                IdFamilia = 1,
                TipoGas = "GAS1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Familias.Add(familia);
                context.SustanciaControlada.Add(sustanciaExistente);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new SustanciaControladaRepository(context);

                // Act
                var resultado = await repository.ValidarSustanciasControladas(0, sustanciaAValidar);

                // Assert
                Assert.True(resultado.Value);
            }
        }

        [Fact]
        public async Task ActualizarSustanciaControlada_NoEncontrada()
        {
            var listaCambios = new List<string> { "Codigo" };
            var sustanciacontroladaActualizada = new  SustanciaControlada { 
                Id = 999,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                IdFamilia =1,
                TipoGas = "GAS1"
            };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarSustanciaControlada(10, listaCambios, sustanciacontroladaActualizada);
            Assert.True(error.IsError);
            Assert.Equal("SustanciaControlada.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Sustancia controlada no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async void CrearSustanciaControlada_Ok()
        {
            //Arrange
            SustanciaControlada sustanciacontrolada = new()
            {
                Id = 5,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                IdFamilia =1,
                TipoGas = "GAS1"
            };


            //Act
            SustanciaControladaRepository repository = await CreateRepositoryAsync();
         
            var result = await repository.CrearSustanciaControlada(sustanciacontrolada);

            //Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task EliminarProvincal_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarSustanciaControlada(999);
            Assert.True(error.IsError);
            Assert.Equal("SustanciaControlada.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Sustancia controlada no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarSustanciaControlada_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarSustanciaControlada(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task SustanciaControladaDuplicada_DevuelveTrue()
        {

            // Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var sustanciacontrolada = new SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                IdFamilia =1,
                TipoGas = "GAS1"
            };

            var sustanciacontrolada2 = new SustanciaControlada()
            {
                Id = 3,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                IdFamilia =1,
                TipoGas = "GAS1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Familias.Add(familia);
                context.SustanciaControlada.Add(sustanciacontrolada);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new SustanciaControladaRepository(context);
                var result = await repository.ObtenerFamiliaSustanciaControlada(1);

                Assert.Equal(1, result.Value.Id);                
            }
        }

        [Fact]
        public async Task SustanciaControladaDuplicada_DevuelveFalse()
        {

            // Arrange
            var familia = new Familia { Id = 1, Nombre="Familia 1" };
            var sustanciacontrolada = new SustanciaControlada()
            {
                Id = 5,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                IdFamilia =1,
                TipoGas = "GAS1"
            };

            // Arrange
            var sustanciacontrolada2 = new SustanciaControlada()
            {
                Id = 8,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                IdFamilia =1,
                TipoGas = "GAS1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Familias.Add(familia);
                context.SustanciaControlada.Add(sustanciacontrolada);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new SustanciaControladaRepository(context);
                var result = await repository.ObtenerFamiliaSustanciaControlada(familia.Id);

                Assert.Equal(1,result.Value.Id);
            }
        }

        [Fact]
        public async Task SustanciaControladaDuplicada_DevuelveError()
        {
            // Arrange            
            var familia = new Familia { Id = 1, Nombre="Familia 1" };
            var sustanciacontroladaCrear = new SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                IdFamilia =1,
                TipoGas = "GAS1"
            };

            // Arrange
            var sustanciacontroladaExiste = new SustanciaControlada
            {
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                IdFamilia =1,
                TipoGas = "GAS1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.SustanciaControlada.Add(sustanciacontroladaCrear);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new SustanciaControladaRepository(context);
                var result = await repository.ObtenerFamiliaSustanciaControlada(sustanciacontroladaExiste.IdFamilia);
                Assert.True(result.IsError);
                Assert.Equal("SustanciaControlada.FamiliaNoEncontrada", result.Errors[0].Code);
            }
        }

    }
}
