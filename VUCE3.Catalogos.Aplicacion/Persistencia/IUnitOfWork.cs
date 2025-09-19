using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Aplicacion.Persistencia
{
    public interface IUnitOfWork
    {
        public Task Save();

        public ICatalogosRepository CatalogosRepository { get; }
        public IVariedadesRepository VariedadesRepository { get; }
        public ICultivosRepository CultivosRepository { get; }
        public IPaisesRepository PaisesRepository { get; }
        public IEmpresasRepository EmpresasRepository { get; }
        public IProfesionalesRepository ProfesionalesRepository { get; }
        public ICasaRepository CasaRepository { get; }
        public IAduanasRepository AduanasRepository { get; }
        public IClientesRepository ClientesRepository { get; }
        public INoticiasVuceRepository NoticiasVuceRepository { get; }       
        public IImagenesRepository ImagenesRepository { get; }
        public IExcepcionesMorosidadRepository ExcepcionesMorosidadRepository { get; }       
        public ITarifasRepository TarifasRepository { get; }
        public IMonedasRepository MonedasRepository {  get; }
        public ITiposAccionesExcepcionesMorosidadRepository TiposAccionesExcepcionesMorosidadRepository { get; }
        public IEstadosExcepcionesMorosidadRepository EstadosExcepcionesMorosidadRepository { get; }
        public ISectoresRepository SectoresRepository { get; }
        public IProvinciaRepository ProvinciaRepository { get; }
        public ICaracteristicasRepository CaracteristicasRepository { get; }
        public IFamiliaRepository FamiliaRepository { get; }
        public ICategoriaRepository CategoriaRepository { get; }
        public IBloqueComercialRepository BloqueComercialRepository { get; }
        public ITipoProductoRepository TipoProductoRepository { get; }
        public ICantonesRepository CantonesRepository { get; }
        public IDistritosRepository DistritosRepository { get; }
        public ISustanciasRepository SustanciasRepository { get; }

        public IRequisitosRepository RequisitosRepository { get; }
        public IBarriosRepository BarriosRepository { get; }
        public ICaracteristicaTipoProductoRepository CaracteristicaTipoProductoRepository { get; }
        public IProductoRequisitoRepository ProductoRequisitoRepository { get; }
        public IPaisBloqueComercialRepository PaisBloqueComercialRepository { get; }
        public IProductosRepository ProductosRepository { get; }
        public ISustanciaControladaRepository SustanciaControladaRepository { get; }
        public IEstablecimientosRepository EstablecimientosRepository { get; }
    }
}




