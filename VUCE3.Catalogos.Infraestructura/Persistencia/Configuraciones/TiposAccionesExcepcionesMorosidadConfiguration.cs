using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class TiposAccionesExcepcionesMorosidadConfiguration : IEntityTypeConfiguration<TipoAccionExcepcionMorosidad>
    {
        public void Configure(EntityTypeBuilder<TipoAccionExcepcionMorosidad> builder)
        {
            builder.ToTable("TiposAccionesExcepcionesMorosidad");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id);
            builder.Property(p => p.Nombre).HasMaxLength(50);
        }
    }
}
