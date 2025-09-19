using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class PaisBloqueComercialConfiguration : IEntityTypeConfiguration<PaisBloqueComercial>
    {
        public void Configure(EntityTypeBuilder<PaisBloqueComercial> builder)
        {
            builder.ToTable("PaisBloqueComercial");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.IdBloqueComercial)
                .HasColumnName("IdBloqueComercialPaisId")
                .IsRequired();
            builder.Property(p => p.IdPais)
                .HasColumnName("IdPais")
                .IsRequired();
        }
    }
}
