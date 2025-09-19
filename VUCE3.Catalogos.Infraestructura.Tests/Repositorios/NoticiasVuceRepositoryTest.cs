using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class NoticiasVuceRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;

        public NoticiasVuceRepositoryTest()
        {
            var dbName = $"BDTestNoticiasVuce_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;        
        }        

        private async Task<NoticiasVuceRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new NoticiasVuceRepository(context);
        }

        [Fact]
        public async Task ObtenerNoticiasVuce_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var noticiasVuce = repository.ObtenerNoticiasVuce();
            var result = Assert.IsType<List<NoticiasVuce>>(noticiasVuce?.Result.Value);

            //Assert
            Assert.Equal("Noticia Vuce Titulo 1", result[0].Titulo);            
        }

        [Fact]
        public async Task ObtenerNoticiasVucePorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var noticiasVuce = repository.ObtenerNoticiasVucePorId(1);
            var result = noticiasVuce?.Result.Value;

            //Assert            
            Assert.Equal("Noticia Vuce Titulo 1", result?.Titulo);            
        }

        [Fact]
        public async Task ObtenerNoticiasVucePorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var noticiasVuce = repository.ObtenerNoticiasVucePorId(10);
            var result = noticiasVuce?.Result.Errors;

            //Assert            
            Assert.True(noticiasVuce?.Result.IsError);
            Assert.Equal("NoticiasVuce.NoEncontrada", result?[0].Code);
            Assert.Equal("NoticiasVuce no encontrada", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarNoticiasVuce_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var noticiasVuce = await repository.ObtenerNoticiasVucePorId(id);
            noticiasVuce.Value.Titulo = "Noticia Vuce Titulo 1";

            //Act
            var noticiasVuceActualizada = await repository.ActualizarNoticiasVuce(noticiasVuce.Value, 1, listaCambios);

            //Assert
            Assert.Equal("Noticia Vuce Titulo 1", noticiasVuceActualizada.Value.Titulo);
        }

        [Fact]
        public async Task ActualizarNoticasVUce_NoEncontrada()
        {
            var listaCambios = new List<string> { "Titulo" };
            var noticiasVuceActualizada = new NoticiasVuce { Id = 999, Titulo = "Noticia Vuce Titulo 1" };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarNoticiasVuce(noticiasVuceActualizada, 999, listaCambios);
            Assert.True(error.IsError);
            Assert.Equal("NoticiasVuce.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("NoticiasVuce no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task CrearNoticiasVuce_Ok()
        {
            var noticiasVuce = new NoticiasVuce
            {
                Id = 4,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            var repository = await CreateRepositoryAsync();

            //Act
            var noticiasVuceNueva = await repository.CrearNoticiasVuce(noticiasVuce);

            //Assert
            var result = Assert.IsType<ErrorOr<NoticiasVuce>>(noticiasVuceNueva);
            Assert.Equal(4, result.Value.Id);
            Assert.Equal("Noticia Vuce Titulo 1", result.Value.Titulo);            
        }

        [Fact]
        public async Task EliminarNoticiasVuce_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarNoticiasVuce(999);
            Assert.True(error.IsError);
            Assert.Equal("NoticiasVuce.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("NoticiasVuce no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarNoticiasVuce_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarNoticiasVuce(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task NoticiasVuceDuplicada_DevuelveOk()
        {

            // Arrange
            var noticiasVuce = new NoticiasVuce
            {
                Id = 4,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarNoticiasVuce(noticiasVuce.Titulo,noticiasVuce.Texto,noticiasVuce.Enlace,noticiasVuce.TituloIngles, noticiasVuce.TextoIngles);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task NoticiasVuceDuplicada_DevuelveError()
        {
            // Arrange
            var noticiasVuce = new NoticiasVuce
            {
                Id = 4,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarNoticiasVuce(noticiasVuce.Titulo, noticiasVuce.Texto, noticiasVuce.Enlace, noticiasVuce.TituloIngles, noticiasVuce.TextoIngles);

            Assert.True(result.Value);
            Assert.Equal("ErrorOr.NoErrors", result.Errors[0].Code);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            var noticiasVuce1 = new NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };


            var noticiasVuce2 = new NoticiasVuce()
            {
                Id = 2,
                Titulo = "Noticia Vuce Titulo 2",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };
            var noticiasVuce3 = new NoticiasVuce()
            {
                Id = 3,
                Titulo = "Noticia Vuce Titulo 3",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            context.NoticiasVuces.Add(noticiasVuce1);
            context.NoticiasVuces.Add(noticiasVuce2);
            context.NoticiasVuces.Add(noticiasVuce3);            

            await context.SaveChangesAsync();
        }

    }
}
