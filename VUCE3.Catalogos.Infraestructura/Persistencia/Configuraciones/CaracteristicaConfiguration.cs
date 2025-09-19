using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class CaracteristicaConfiguration : IEntityTypeConfiguration<Caracteristica>
    {
        public void Configure(EntityTypeBuilder<Caracteristica> builder)
        {
            builder.ToTable("Caracteristicas");            
            builder.HasKey(p => p.Id);
            builder.Property(p => p.IdInstitucion)
                .HasColumnName("Responsible government agency identifier");
            builder.Property(p => p.Nombre)
                .HasMaxLength(150)
                .HasColumnName("Commodity characteristics description");
        }
    }
}
