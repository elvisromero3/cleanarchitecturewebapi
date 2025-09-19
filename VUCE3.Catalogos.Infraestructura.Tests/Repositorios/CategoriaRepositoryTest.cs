using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;
using VUCE3.Catalogos.Infraestructura.Persistencia;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class CategoriaRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public CategoriaRepositoryTest()
        {
            var dbName = $"BDTestDCategorias_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<CategoriaRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new CategoriaRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            Categoria Categoria = new()
            {
                Id = 1,
                Nombre = "Categoria 1"
            };

            context.Categorias.Add(Categoria);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerCategorias_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var Categorias = repository.ObtenerCategorias();
            var result = Assert.IsType<List<Categoria>>(Categorias?.Result.Value);

            //Assert
            Assert.Equal("Categoria 1", result[0].Nombre);
        }

        [Fact]
        public async Task ObtenerCategoriaPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var Categorias = repository.ObtenerCategoriaPorId(1);
            var result = Categorias?.Result.Value;

            //Assert            
            Assert.Equal("Categoria 1", result?.Nombre);
        }

        [Fact]
        public async Task ObtenerCategoriaPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var Categoria = repository.ObtenerCategoriaPorId(10);
            var result = Categoria?.Result.Errors;

            //Assert            
            Assert.True(Categoria?.Result.IsError);
            Assert.Equal("Categoria.NoEncontrada", result?[0].Code);
            Assert.Equal("Categoria no encontrada", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarCategoria_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var Categoria = await repository.ObtenerCategoriaPorId(id);
            Categoria.Value.Nombre = "Categoria 2";

            //Act
            var CategoriaActualizada = await repository.ActualizarCategoria(1, listaCambios, Categoria.Value);

            //Assert
            Assert.Equal("Categoria 2", CategoriaActualizada.Value.Nombre);
        }

        [Fact]
        public async Task ActualizarCategoria_NoEncontrada()
        {
            var listaCambios = new List<string> { "Nombre" };
            var CategoriaActualizada = new Categoria { Id = 999, Nombre = "Nombre Categoria" };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarCategoria(10, listaCambios, CategoriaActualizada);
            Assert.True(error.IsError);
            Assert.Equal("Categoria.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Categoria no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async void CrearCategorias_Ok()
        {
            //Arrange
            Categoria Categoria = new()
            {
                Id = 5,
                Nombre = "Categoria 1"
            };


            //Act
            CategoriaRepository repository = await CreateRepositoryAsync();

            var result = await repository.CrearCategoria(Categoria);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarCategoria_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarCategoria(999);
            Assert.True(error.IsError);
            Assert.Equal("Categoria.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Categoria no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarCategoria_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarCategoria(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task CategoriaDuplicada_DevuelveTrue()
        {

            // Arrange
            var Categoria = new Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 5
            };

            var Categoria2 = new Categoria()
            {
                Id = 3,
                Nombre = "Categoria 1", 
                IdInstitucion = 5
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Categorias.Add(Categoria);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CategoriaRepository(context);
                var result = await repository.ValidarCategoria(Categoria2.Id, Categoria2.Nombre, Categoria2.IdInstitucion);

                Assert.True(result.Value);
            }
        }

        [Fact]
        public async Task CategoriaDuplicada_DevuelveFalse()
        {

            // Arrange
            var Categoria = new Categoria()
            {
                Id = 5,
                Nombre = "Categoria 2",
                IdInstitucion= 5                
            };

            // Arrange
            var Categoria2 = new Categoria()
            {
                Id = 8,
                Nombre = "Categoria 3",
                IdInstitucion = 5
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Categorias.Add(Categoria);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CategoriaRepository(context);
                var result = await repository.ValidarCategoria(Categoria2.Id, Categoria2.Nombre, Categoria2.IdInstitucion);

                Assert.False(result.Value);
            }
        }

        [Fact]
        public async Task CategoriaDuplicada_DevuelveError()
        {
            // Arrange            

            var CategoriaCrear = new Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 5
            };

            // Arrange
            var CategoriaExiste = new Categoria
            {
                Nombre = "Categoria 1",
                IdInstitucion = 5
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Categorias.Add(CategoriaCrear);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CategoriaRepository(context);
                var result = await repository.ValidarCategoria(CategoriaExiste.Id, CategoriaExiste.Nombre, CategoriaExiste.IdInstitucion);
                Assert.True(result.Value);
                Assert.Equal("ErrorOr.NoErrors", result.Errors[0].Code);
            }
        }
    }
}
