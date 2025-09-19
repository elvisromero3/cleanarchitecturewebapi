using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    public class SustanciaConfiguration : IEntityTypeConfiguration<Sustancia>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Sustancia> builder)
        {
            builder.ToTable("Sustancias");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nombre)
                .HasColumnName("Commodity characteristics description-Name")
                .HasMaxLength(300)
                .IsRequired();
            builder.Property(e => e.Cas)
                .HasColumnName("Commodity characteristics description")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(e => e.ListaCaq)
                .HasColumnName("Commodity characteristic code")
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
