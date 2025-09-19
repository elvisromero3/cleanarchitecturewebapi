using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class FamiliaConfiguration : IEntityTypeConfiguration<Familia>
    {
        public void Configure(EntityTypeBuilder<Familia> builder)
        {
            builder.ToTable("Familias");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Nombre)
                .HasMaxLength(50)
                .HasColumnName("Commodity category code");

        }
    }
}
