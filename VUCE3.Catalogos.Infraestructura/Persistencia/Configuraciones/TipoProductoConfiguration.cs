using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class TipoProductoConfiguration : IEntityTypeConfiguration<TipoProducto>
    {
        public void Configure(EntityTypeBuilder<TipoProducto> builder)
        {
            builder.ToTable("TipoProductos");
            builder.HasKey(p => p.Id);          
            builder.Property(p => p.IdCategoria)
                .HasColumnName("Intended use statement")
               .IsRequired();
            builder.Property(p => p.Tipo)
                .HasColumnName("Commodity category code")
               .IsRequired()
               .HasMaxLength(200);
            builder.Property(p => p.IdInstitucion)
                .HasColumnName("Responsible government agency identifier")
               .IsRequired();

        }

    }
}
