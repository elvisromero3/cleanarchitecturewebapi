using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class CantonesConfiguration : IEntityTypeConfiguration<Canton>
    {
        public void Configure(EntityTypeBuilder<Canton> builder)
        {
            builder.ToTable("Cantones");
            builder.HasKey(p => p.Id);          
            builder.Property(p => p.Nombre)
                .HasMaxLength(50)
                .HasColumnName("Country sub-entity identification code")
                .IsRequired();
            builder.Property(p => p.Codigo)
                .HasMaxLength(3)
                .HasColumnName("Country sub-entity identification code-CC")
                .IsRequired();
            builder.Property(p => p.IdProvincia)
                .HasColumnName("Country sub-entity name");

            builder.HasOne(c => c.Provincia)
                .WithMany(p => p.Cantones)
                .HasForeignKey(c=>c.IdProvincia)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
