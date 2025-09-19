using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class ExcepcionMorosidadConfiguration : IEntityTypeConfiguration<ExcepcionMorosidad>
    {
        public void Configure(EntityTypeBuilder<ExcepcionMorosidad> builder)
        {
            builder.ToTable("ExcepcionesMorosidad");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.IdTipoAccion)
                .HasColumnName("Additional statement code");
            builder.Property(p => p.TipoIdentificacionEmpresa)
                .HasColumnName("Role code");
            builder.Property(p => p.NombreEmpresa)
                .HasColumnName("Master data party name")
                .HasMaxLength(100);
            builder.Property(p => p.NumeroIdentificacionEmpresa)
                .HasColumnName("Additional identifier")
                .HasMaxLength(12);
            builder.Property(p => p.FechaInicio)
                .HasColumnName("Document effective date/time");
            builder.Property(p => p.FechaVencimiento)
                .HasColumnName("Document expiration date/time");
            builder.Property(p => p.Observaciones)
                .HasColumnName("Additional statement description")
                .HasMaxLength(250);

            builder.HasOne(e => e.TipoAccionExcepcionMorosidad)
               .WithMany(e => e.ExcepcionesMorosidad)
               .HasForeignKey("IdTipoAccion");

            builder.HasOne(e => e.EstadoExcepcionMorosidad)
               .WithMany(e => e.ExcepcionesMorosidad)
               .HasForeignKey("IdEstado");
        }
    }
}
