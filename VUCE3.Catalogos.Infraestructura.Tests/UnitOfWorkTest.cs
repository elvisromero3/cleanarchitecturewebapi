using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;


namespace VUCE3.Catalogos.Infraestructura.Tests
{
    public class UnitOfWorkTest
    {
        [Fact]
        public async Task ObtenerRepositorios_DevuelveOk()
        {
            var mockContext = new Mock<CatalogosDbContext>(new DbContextOptions<CatalogosDbContext>());
            mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None));

            var mockConfig = new Mock<IConfiguration>();
            var mockBlob = new Mock<BlobServiceClient>();

            var unitOfWork = new UnitOfWork(mockContext.Object, mockConfig.Object, mockBlob.Object);
            await unitOfWork.Save();

            //Assert
            Assert.IsType<CatalogosRepository>(unitOfWork.CatalogosRepository);
            Assert.IsType<VariedadesRepository>(unitOfWork.VariedadesRepository);
            Assert.IsType<CultivosRepository>(unitOfWork.CultivosRepository);
            Assert.IsType<PaisesRepository>(unitOfWork.PaisesRepository);
            Assert.IsType<ProfesionalesRepository>(unitOfWork.ProfesionalesRepository);
            Assert.IsType<EmpresasRepository>(unitOfWork.EmpresasRepository);
            Assert.IsType<CasaRepository>(unitOfWork.CasaRepository);
            Assert.IsType<AduanasRepository>(unitOfWork.AduanasRepository);
            Assert.IsType<ImagenesRepository>(unitOfWork.ImagenesRepository);
            Assert.IsType<ClientesRepository>(unitOfWork.ClientesRepository);
            Assert.IsType<NoticiasVuceRepository>(unitOfWork.NoticiasVuceRepository);
            Assert.IsType<ExcepcionesMorosidadRepository>(unitOfWork.ExcepcionesMorosidadRepository);     
            Assert.IsType<MonedasRepository>(unitOfWork.MonedasRepository);            
            Assert.IsType<TarifasRepository>(unitOfWork.TarifasRepository);
            Assert.IsType<EstadosExcepcionesMorosidadRepository>(unitOfWork.EstadosExcepcionesMorosidadRepository);
            Assert.IsType<TiposAccionesExcepcionesMorosidadRepository>(unitOfWork.TiposAccionesExcepcionesMorosidadRepository);
            Assert.IsType<SectoresRepository>(unitOfWork.SectoresRepository);
            Assert.IsType<ProvinciaRepository>(unitOfWork.ProvinciaRepository);
            Assert.IsType<CaracteristicasRepository>(unitOfWork.CaracteristicasRepository);
            Assert.IsType<FamiliaRepository>(unitOfWork.FamiliaRepository);
            Assert.IsType<CategoriaRepository>(unitOfWork.CategoriaRepository);
            Assert.IsType<BloqueComercialRepository>(unitOfWork.BloqueComercialRepository);
            Assert.IsType<TipoProductoRepository>(unitOfWork.TipoProductoRepository);
            Assert.IsType<CantonesRepository>(unitOfWork.CantonesRepository);
            Assert.IsType<SustanciasRepository>(unitOfWork.SustanciasRepository);
            Assert.IsType<DistritosRepository>(unitOfWork.DistritosRepository);
            Assert.IsType<RequisitosRepository>(unitOfWork.RequisitosRepository);
            Assert.IsType<BarriosRepository>(unitOfWork.BarriosRepository);
            Assert.IsType<CaracteristicaTipoProductoRepository>(unitOfWork.CaracteristicaTipoProductoRepository);
            Assert.IsType<ProductoRequisitoRepository>(unitOfWork.ProductoRequisitoRepository);
            Assert.IsType<PaisBloqueComercialRepository>(unitOfWork.PaisBloqueComercialRepository);
            Assert.IsType<SustanciaControladaRepository>(unitOfWork.SustanciaControladaRepository);
        }
    }
}
