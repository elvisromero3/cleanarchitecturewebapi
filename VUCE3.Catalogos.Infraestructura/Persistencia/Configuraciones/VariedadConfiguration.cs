using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class VariedadConfiguration : IEntityTypeConfiguration<Variedad>
    {
        public void Configure(EntityTypeBuilder<Variedad> builder)
        {
            builder.ToTable("Variedades");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Codigo)
                .HasColumnName("Commodity characteristic code")
                .HasMaxLength(17)
                .IsRequired();
            builder.Property(p => p.Nombre)
                .HasColumnName("Commodity characteristics description")
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
