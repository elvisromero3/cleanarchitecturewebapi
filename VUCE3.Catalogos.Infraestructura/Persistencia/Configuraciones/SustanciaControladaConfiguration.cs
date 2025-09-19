using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class SustanciaControladaConfiguration : IEntityTypeConfiguration<SustanciaControlada>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<SustanciaControlada> builder)
        {
            builder.ToTable("SustanciaControlada");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.ClasificacionArancelaria)
                .HasColumnName("HSClassification")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(e => e.ClasificacionAshrae)
                .HasColumnName("Commodity characteristic code")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(e => e.PotencialCalentamientoGlobal)
                .HasColumnName("Additional statement description")
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(e => e.TipoGas)
                .HasColumnName("Commodity characteristics description")
                .HasMaxLength(100);
            builder.Property(e => e.IdFamilia)
                .HasColumnName("Commodity category code");

            builder.HasOne(e => e.Familia)
                .WithMany(e => e.SustanciaControlada)
                .HasForeignKey(e=>e.IdFamilia)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
            

            builder.Navigation(e => e.Familia).AutoInclude();
        }
    }
}
