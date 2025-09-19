using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Moq;
using Azure.Storage.Blobs.Models;
using Azure;
namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class RequisitosRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public RequisitosRepositoryTest()
        {
            var dbName = $"BDTestRequisitos_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<RequisitosRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new CatalogosDbContext(dbContextOptions);

            var inMemorySettings = new Dictionary<string, string?> {
                {"Blob:ContainerName", "name"},
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var mockBlobContainer = new Mock<BlobContainerClient>();

            var mockBlobClient = new Mock<BlobClient>();
            mockBlobClient
                .Setup(m => m.DeleteAsync(DeleteSnapshotsOption.None, null, default))
                .Returns(It.IsAny<Task<Azure.Response>>());
            var responseMock = new Mock<Response>();
            mockBlobClient
                .Setup(m => m.UploadAsync(It.IsAny<Stream>(), It.IsAny<bool>(), default).Result)
                .Returns(Response.FromValue<BlobContentInfo>(
                    BlobsModelFactory.BlobContentInfo(new ETag(), new DateTimeOffset(), [], "", "", "", 0),
                    responseMock.Object));
            mockBlobClient
                .Setup(m => m.DeleteAsync(DeleteSnapshotsOption.None, null, default).Result);

            var mockBlobService = new Mock<BlobServiceClient>();
            mockBlobService.Setup(m => m.GetBlobContainerClient(It.IsAny<string>())).Returns(mockBlobContainer.Object);
            mockBlobContainer.Setup(m => m.GetBlobClient(It.IsAny<string>())).Returns(mockBlobClient.Object);

            await PopulateDataAsync(context);
            return new RequisitosRepository(context, configuration, mockBlobService.Object);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {
            Requisito requisito = new()
            {
                Id = 1,                
                Codigo = "01",
                Descripcion = "Descripción",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "332c4193-e4bc-412e-802b-25770d30d13c",
                NombreImagenRequisito = "avatar.jpeg"
            };
            
            context.Requisitos.Add(requisito);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task ObtenerRequisitos_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var requisitos = repository.ObtenerRequisitos();
            var result = Assert.IsType<List<Requisito>>(requisitos?.Result.Value);

            //Assert
            Assert.Equal("Descripción", result[0].Descripcion);
        }

        [Fact]
        public async Task ObtenerRequisitoPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var requisito = repository.ObtenerRequisitoPorId(1);
            var result = requisito?.Result.Value;

            //Assert            
            Assert.Equal("Descripción", result?.Descripcion);
        }

        [Fact]
        public async Task ObtenerRequisitoPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var requisito = repository.ObtenerRequisitoPorId(10);
            var result = requisito?.Result.Errors;

            //Assert            
            Assert.True(requisito?.Result.IsError);
            Assert.Equal("Requisito.NoEncontrado", result?[0].Code);
            Assert.Equal("Requisito no encontrado", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarRequisito_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Descripcion", "ImagenRequisito", "NombreImagenRequisito" };
            var repository = await CreateRepositoryAsync();

            var requisito = await repository.ObtenerRequisitoPorId(id);
            requisito.Value.Descripcion = "Descripción 2";
            requisito.Value.Codigo = "01";
            requisito.Value.ImagenRequisito = testFile;
            requisito.Value.NombreImagenRequisito = "prueba.jpeg";

            //Act
            var requisitoActualizado = await repository.ActualizarRequisito(requisito.Value, id, listaCambios);

            //Assert
            Assert.Equal("Descripción 2", requisitoActualizado.Value.Descripcion);
        }

        [Fact]
        public async Task ActualizarRequisito_NoEncontrado()
        {
            var listaCambios = new List<string> { "Codigo" };
            var requisitoActualizado = new Requisito
            {
                Id = 999,
                Codigo = "01",
                Descripcion = "Descripción",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarRequisito(requisitoActualizado, 999, listaCambios);
            Assert.True(error.IsError);
            Assert.Equal("Requisito.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Requisito no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async void CrearRequisito_Ok()
        {
            //Arrange
            Requisito requisito = new()
            {
                Id = 5,
                Codigo = "05",
                Descripcion = "Descripción",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = testFile,
                NombreImagenRequisito = "prueba.jpeg"
            };

            RequisitosRepository repository = await CreateRepositoryAsync();

            //Act
            var result = await repository.CrearRequisito(requisito);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarRequisito_NoEncontrado()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarRequisito(999);
            Assert.True(error.IsError);
            Assert.Equal("Requisito.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Requisito no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarRequisito_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarRequisito(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task ValidarRequisito_True()
        {
            var repository = await CreateRepositoryAsync();
            var result = await repository.ValidarRequisito(2, "01", "Version 1", 1, 1);

            Assert.False(result.IsError);
            Assert.True(result.Value);
        }

        private static string testFile = "data:image/jpeg;base64,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";
    }
}
