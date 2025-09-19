using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class ProvinciaConfiguration : IEntityTypeConfiguration<Provincia>
    {
        public void Configure(EntityTypeBuilder<Provincia> builder)
        {
            builder.ToTable("Provincias");
            builder.HasKey(p => p.Id);          
            builder.Property(p => p.Nombre)
                .HasMaxLength(50)
                .HasColumnName("Country sub-entity name")
                .IsRequired();
            builder.Property(p => p.Codigo)
                .HasMaxLength(1)
                .HasColumnName("Country sub-entity name-CP")
                .IsRequired();
        }

    }
}
