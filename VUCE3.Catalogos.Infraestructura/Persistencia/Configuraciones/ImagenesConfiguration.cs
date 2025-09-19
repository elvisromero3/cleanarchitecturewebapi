using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class ImagenesConfiguration : IEntityTypeConfiguration<Imagenes>
    {
        public void Configure(EntityTypeBuilder<Imagenes> builder)
        {
            builder.ToTable("Imagenes");
            builder.HasKey(p => p.Id);            
            builder.Property(p => p.Imagen)
                .IsRequired()
                .HasColumnType("nvarchar(MAX)");
        }
    }
}
