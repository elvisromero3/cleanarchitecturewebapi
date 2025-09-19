using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class BloqueComercialConfiguration : IEntityTypeConfiguration<BloqueComercial>
    {
        public void Configure(EntityTypeBuilder<BloqueComercial> builder)
        {
            builder.ToTable("BloquesComerciales");
            builder.HasKey(p => p.Id);          
            builder.Property(p => p.Nombre).HasMaxLength(300)
               .IsRequired();
        }
    }
}
