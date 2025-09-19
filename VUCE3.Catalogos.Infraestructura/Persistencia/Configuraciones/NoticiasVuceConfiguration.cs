using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class NoticiasVuceConfiguration : IEntityTypeConfiguration<NoticiasVuce>
    {
        public void Configure(EntityTypeBuilder<NoticiasVuce> builder)
        {
            builder.ToTable("NoticiasVuce");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Titulo)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.Texto)
                .IsRequired()
                .HasColumnType("nvarchar(max)");
            builder.Property(p => p.Enlace)
                .HasMaxLength(250);
            builder.Property(p => p.TituloIngles)
               .IsRequired()
               .HasMaxLength(100);
            builder.Property(p => p.TextoIngles)
               .IsRequired()
               .HasColumnType("nvarchar(max)");
        }
    }
}
