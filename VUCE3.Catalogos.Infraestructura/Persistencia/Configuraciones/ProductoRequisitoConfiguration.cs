using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class ProductoRequisitoConfiguration : IEntityTypeConfiguration<ProductoRequisito>
    {
        public void Configure(EntityTypeBuilder<ProductoRequisito> builder)
        {
            builder.ToTable("ProductoRequisito");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id);
            builder.HasOne(e => e.TipoProducto)
               .WithMany(e => e.ProductoRequisito)
               .HasForeignKey("IdTipoProducto");

            builder.HasOne(e => e.Requisito)
                .WithMany(e => e.ProductoRequisito)
                .HasForeignKey("IdRequisito");
        }
    }
}
