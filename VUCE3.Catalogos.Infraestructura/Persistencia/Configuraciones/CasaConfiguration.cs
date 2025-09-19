using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class CasaConfiguration : IEntityTypeConfiguration<Casa>
    {
        public void Configure(EntityTypeBuilder<Casa> builder)
        {
            builder.ToTable("Casas");
            builder.HasKey(p => p.Id);          
            builder.Property(p => p.Codigo)
                .HasColumnName("StatementCode")
                .HasMaxLength(17)
               .IsRequired();
            builder.Property(p => p.Nombre)
                .HasColumnName("StatementDescription")
                .IsRequired()
                .HasMaxLength(100);

        }

    }
}
