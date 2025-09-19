using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia
{
    public class CatalogosDbContext : DbContext
    {
        public CatalogosDbContext(DbContextOptions<CatalogosDbContext> options) : base(options)
        {
        }

        public DbSet<Catalogo> Catalogos { get; set; }
        public DbSet<Variedad> Variedades { get; set; }
        public DbSet<Cultivo> Cultivos { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Profesional> Profesionales { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Casa> Casas { get; set; }
        public DbSet<Aduana> Aduanas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<NoticiasVuce> NoticiasVuces { get; set; }
        public DbSet<Imagenes> Imagenes { get; set; }
        public DbSet<CatalogoInstitucion> CatalogoInstitucion { get; set; }
        public DbSet<ExcepcionMorosidad> ExcepcionesMorosidad { get; set; }
        public DbSet<Tarifa> Tarifas { get; set; }
        public DbSet<Moneda> Monedas { get; set; }
        public DbSet<TipoAccionExcepcionMorosidad> TiposAccionesExcepcionesMorosidad { get; set; }
        public DbSet<EstadoExcepcionMorosidad> EstadosExcepcionesMorosidad { get; set; }
        public DbSet<Sector> Sectores { get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Caracteristica> Caracteristicas { get; set; }
        public DbSet<Familia> Familias { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<BloqueComercial> BloquesComerciales { get; set; }
        public DbSet<TipoProducto> TipoProductos { get; set; }
        public DbSet<Canton> Cantones { get; set; }
        public DbSet<Distrito> Distritos { get; set; }
        public DbSet<Sustancia> Sustancias { get; set; }
        public DbSet<Requisito> Requisitos { get; set; }
        public DbSet<Barrio> Barrios { get; set; }
        public DbSet<CaracteristicaTipoProducto> CaracteristicaTipoProductos { get; set; }
        public DbSet<ProductoRequisito> ProductoRequisitos { get; set; }
        public DbSet<PaisBloqueComercial> PaisBloqueComercial { get; set; }
        public DbSet<SustanciaControlada> SustanciaControlada { get; set; }
        public DbSet<Establecimiento> Establecimientos { get; set; }

        public DbSet<Productos> Productos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogosDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}