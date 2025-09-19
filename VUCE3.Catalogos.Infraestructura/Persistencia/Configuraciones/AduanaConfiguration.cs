using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class AduanaConfiguration : IEntityTypeConfiguration<Aduana>
    {
        public void Configure(EntityTypeBuilder<Aduana> builder)
        {
            builder.ToTable("Aduanas");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Nombre).HasColumnName("DeclarationOffice").HasMaxLength(100);
        }
    }
}
