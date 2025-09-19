using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class PaisConfiguration : IEntityTypeConfiguration<Pais>
    {
        public void Configure(EntityTypeBuilder<Pais> builder)
        {
            builder.ToTable("Paises");
            builder.HasKey(p => p.Id);            

            builder.Property(p => p.Nombre)
                .HasColumnName("CountryName")
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.CodigoA2)
                .HasColumnName("CountryCode")
                .IsRequired()
                .HasMaxLength(2);
            builder.Property(p => p.CodigoNumerico)
                .HasColumnName("CountryCodeN3")
                .IsRequired()
                .HasMaxLength(3);
            builder.Property(p => p.CodigoC3)
                .HasColumnName("CountryCodeC3")
                .IsRequired()
                .HasMaxLength(3);

        }
    }
}
