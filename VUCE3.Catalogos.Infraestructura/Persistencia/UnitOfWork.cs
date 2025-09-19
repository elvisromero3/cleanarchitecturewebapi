using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Persistencia
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CatalogosDbContext _context;
        private readonly IConfiguration _config;
        private readonly BlobServiceClient _blobServiceClient;

        private ICatalogosRepository _catalogosRepository = null!;
        private IVariedadesRepository _variedadesRepository = null!;
        private ICultivosRepository _cultivosRepository = null!;
        private IPaisesRepository _paisesRepository = null!;
        private IEmpresasRepository _empresasRepository = null!;
        private IProfesionalesRepository _profesionalesRepository = null!;
        private ICasaRepository _casaRepository = null!;
        private IClientesRepository _clientesRepository = null!;
        private IAduanasRepository _aduanasRepository = null!;
        private INoticiasVuceRepository _noticiasVuceRepository = null!;
        private ITarifasRepository _tarifasRepository = null!;
        private IMonedasRepository _monedasRepository = null!;
        private ImagenesRepository _imagenesRepository = null!;
        private IExcepcionesMorosidadRepository _excepcionesMorosidadRepository = null!;
        private ITiposAccionesExcepcionesMorosidadRepository _tiposAccionesExcepcionesMorosidadRepository = null!;
        private IEstadosExcepcionesMorosidadRepository _estadosExcepcionesMorosidadRepository = null!;
        private ISectoresRepository _sectoresRepository = null!;
        private IProvinciaRepository _provinciaRepository = null!;
        private ICaracteristicasRepository _caracteristicasRepository = null!;
        private IFamiliaRepository _familiasRepository = null!;
        private ICategoriaRepository _categoriaRepository = null!;
        private IBloqueComercialRepository _bloqueComercialRepository = null!;
        private ITipoProductoRepository  _tipoProductoRepository = null!;
        private ICantonesRepository _cantonesRepository = null!;
        private IDistritosRepository _distritosRepository = null!;
        private ISustanciasRepository _sustanciasRepository = null!;
        private IRequisitosRepository _requisitosRepository = null!;
        private IBarriosRepository _barriosRepository = null!;
        private ICaracteristicaTipoProductoRepository _caracteristicaTipoProductoRepository = null!;
        private IProductoRequisitoRepository _productoRequisitoRepository = null!;
        private IPaisBloqueComercialRepository _paisBloqueComercialRepository = null!;
        private IProductosRepository _productosRepository = null!;
        private ISustanciaControladaRepository _sustanciaControladaRepository = null!;
        private IEstablecimientosRepository _establecimientosRepository = null!;

        public UnitOfWork(CatalogosDbContext context, IConfiguration config, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _config = config;
            _blobServiceClient = blobServiceClient;
        }

        public ICatalogosRepository CatalogosRepository
        {
            get
            {
                if (_catalogosRepository is null)
                {
                    _catalogosRepository = new CatalogosRepository(_context);
                }

                return _catalogosRepository;
            }
        }

        public IVariedadesRepository VariedadesRepository
        {
            get
            {
                if (_variedadesRepository is null)
                {
                    _variedadesRepository = new VariedadesRepository(_context);
                }

                return _variedadesRepository;
            }
        }

        public ICultivosRepository CultivosRepository
        {
            get
            {
                if (_cultivosRepository is null)
                {
                    _cultivosRepository = new CultivosRepository(_context);
                }

                return _cultivosRepository;
            }
        }

        public IPaisesRepository PaisesRepository
        {
            get
            {
                if (_paisesRepository is null)
                {
                    _paisesRepository = new PaisesRepository(_context);
                }

                return _paisesRepository;
            }
        }

        public IEmpresasRepository EmpresasRepository
        {
            get
            {
                if (_empresasRepository is null)
                {
                    _empresasRepository = new EmpresasRepository(_context);
                }

                return _empresasRepository;
            }
        }

        public IProfesionalesRepository ProfesionalesRepository
        {
            get
            {
                if (_profesionalesRepository is null)
                {
                    _profesionalesRepository = new ProfesionalesRepository(_context);
                }

                return _profesionalesRepository;

            }
        }
        
        public ICasaRepository CasaRepository
        {
            get
            {
                if (_casaRepository is null)
                {
                    _casaRepository = new CasaRepository(_context);
                }
                return _casaRepository;
            }
        }

        public IClientesRepository ClientesRepository
        {
            get
            {
                if (_clientesRepository is null)
                {
                    _clientesRepository = new ClientesRepository(_context);
                }
                return _clientesRepository;
            }
        }

        public IAduanasRepository AduanasRepository
        {
            get
            {
                if (_aduanasRepository is null)
                {
                    _aduanasRepository = new AduanasRepository(_context);
                }
                return _aduanasRepository;
            }
        }

        public INoticiasVuceRepository NoticiasVuceRepository
        {
            get
            {
                if (_noticiasVuceRepository is null)
                {
                    _noticiasVuceRepository = new NoticiasVuceRepository(_context);
                }
                return _noticiasVuceRepository;
            }
        }

        public ITarifasRepository TarifasRepository
        {
            get
            {
                if (_tarifasRepository is null)
                {
                    _tarifasRepository = new TarifasRepository(_context);
                }
                return _tarifasRepository;
            }
        }

        public IMonedasRepository MonedasRepository
        {
            get
            {
                if (_monedasRepository is null)
                {
                    _monedasRepository = new MonedasRepository(_context);
                }
                return _monedasRepository;
            }
        }

        public IImagenesRepository ImagenesRepository
        {
            get
            {
                if (_imagenesRepository is null)
                {
                    _imagenesRepository = new ImagenesRepository(_context, _config, _blobServiceClient);
                }
                return _imagenesRepository;
            }
        }

        public IExcepcionesMorosidadRepository ExcepcionesMorosidadRepository
        {
            get
            {
                if (_excepcionesMorosidadRepository is null)
                {
                    _excepcionesMorosidadRepository = new ExcepcionesMorosidadRepository(_context);
                }
                return _excepcionesMorosidadRepository;
            }
        }

        public ITiposAccionesExcepcionesMorosidadRepository TiposAccionesExcepcionesMorosidadRepository
        {
            get
            {
                if (_tiposAccionesExcepcionesMorosidadRepository is null)
                {
                    _tiposAccionesExcepcionesMorosidadRepository = new TiposAccionesExcepcionesMorosidadRepository(_context);
                }
                return _tiposAccionesExcepcionesMorosidadRepository;
            }
        }

        public IEstadosExcepcionesMorosidadRepository EstadosExcepcionesMorosidadRepository
        {
            get
            {
                if (_estadosExcepcionesMorosidadRepository is null)
                {
                    _estadosExcepcionesMorosidadRepository = new EstadosExcepcionesMorosidadRepository(_context);
                }
                return _estadosExcepcionesMorosidadRepository;
            }
        }

        public ISectoresRepository SectoresRepository
        {
            get
            {
                if (_sectoresRepository is null)
                {
                    _sectoresRepository = new SectoresRepository(_context);
                }

                return _sectoresRepository;
            }
        }
        public IProvinciaRepository ProvinciaRepository
        {
            get
            {
                if (_provinciaRepository is null)
                {
                    _provinciaRepository = new ProvinciaRepository(_context);
                }
                return _provinciaRepository;
            }
        }
        public ISustanciasRepository SustanciasRepository
        {
            get
            {
                if (_sustanciasRepository is null)
                {
                    _sustanciasRepository = new SustanciasRepository(_context);
                }
                return _sustanciasRepository;
            }
        }
        public ICaracteristicasRepository CaracteristicasRepository
        {
            get
            {
                if (_caracteristicasRepository is null)
                {
                    _caracteristicasRepository = new CaracteristicasRepository(_context);
                }
                return _caracteristicasRepository;
            }
        }
        public IBloqueComercialRepository BloqueComercialRepository
        {
            get
            {
                if ( _bloqueComercialRepository is null)
                {
                    _bloqueComercialRepository = new BloqueComercialRepository(_context);
                }
                return _bloqueComercialRepository;
            }
        }


        public IFamiliaRepository FamiliaRepository
        {
            get
            {
                if (_familiasRepository is null)
                {
                    _familiasRepository = new FamiliaRepository(_context);
                }
                return _familiasRepository;
            }
        }

        public ICategoriaRepository CategoriaRepository
        {
            get
            {
                if (_categoriaRepository is null)
                {
                    _categoriaRepository = new CategoriaRepository(_context);
                }
                return _categoriaRepository;
            }
        }
        public ITipoProductoRepository TipoProductoRepository
        {
            get
            {
                if (_tipoProductoRepository is null)
                {
                    _tipoProductoRepository = new TipoProductoRepository(_context);
                }
                return _tipoProductoRepository;
            }
        }
        public ICantonesRepository CantonesRepository
        {
            get
            {
                if (_cantonesRepository is null)
                {
                    _cantonesRepository = new CantonesRepository(_context);
                }
                return _cantonesRepository;
            } 
        }
        public ISustanciaControladaRepository SustanciaControladaRepository
        {
            get
            {
                if (_sustanciaControladaRepository is null)
                {
                    _sustanciaControladaRepository = new SustanciaControladaRepository(_context);
                }
                return _sustanciaControladaRepository;
            }

        }

        public IDistritosRepository DistritosRepository
        {
            get
            {
                if (_distritosRepository is null)
                {
                    _distritosRepository = new DistritosRepository(_context);
                }
                return _distritosRepository;
            }
        }


        public IRequisitosRepository RequisitosRepository
        {
            get
            {
                if (_requisitosRepository is null)
                {
                    _requisitosRepository = new RequisitosRepository(_context, _config, _blobServiceClient);
                }
                return _requisitosRepository;
            }
        }
        public ICaracteristicaTipoProductoRepository CaracteristicaTipoProductoRepository
        {
            get
            {
                if (_caracteristicaTipoProductoRepository is null)
                {
                    _caracteristicaTipoProductoRepository = new CaracteristicaTipoProductoRepository(_context);
                }
                return _caracteristicaTipoProductoRepository;
            }

        }

        public IBarriosRepository BarriosRepository
        {
            get
            {
                if (_barriosRepository is null)
                {
                    _barriosRepository = new BarriosRepository(_context);
                }
                return _barriosRepository;
            }
        }
        public IProductoRequisitoRepository ProductoRequisitoRepository
        {
            get
            {
                if (_productoRequisitoRepository is null)
                {
                    _productoRequisitoRepository = new ProductoRequisitoRepository(_context);
                }
                return _productoRequisitoRepository;
            }
        }
        public IPaisBloqueComercialRepository PaisBloqueComercialRepository
        {
            get
            {
                if (_paisBloqueComercialRepository is null)
                {
                    _paisBloqueComercialRepository = new PaisBloqueComercialRepository(_context);
                }
                return _paisBloqueComercialRepository;
            }
        }

        public IProductosRepository ProductosRepository
        {
            get
            {
                if (_productosRepository is null)
                {
                    _productosRepository = new ProductosRepository(_context);
                }
                return _productosRepository;
            }
        }
        public IEstablecimientosRepository EstablecimientosRepository
        {
            get
            {
                if (_establecimientosRepository is null)
                {
                    _establecimientosRepository = new EstablecimientosRepository(_context);
                }
                return _establecimientosRepository;
            }
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
