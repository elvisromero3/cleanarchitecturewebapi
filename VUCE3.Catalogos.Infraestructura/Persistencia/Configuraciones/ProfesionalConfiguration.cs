using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class ProfesionalConfiguration : IEntityTypeConfiguration<Profesional>
    {
        public void Configure(EntityTypeBuilder<Profesional> builder)
        {
            builder.ToTable("Profesionales");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("IdInterno");
            builder.Property(p => p.Nombre)
                .HasColumnName("Name")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(p => p.IdTipoIdentificacion)
                .HasColumnName("ID.IdentificationScheme.Identifier")
                .IsRequired();
            builder.Property(p => p.NumeroIdentificacion)
                .HasColumnName("ID")
                .HasMaxLength(12)
                .IsRequired();
            builder.Property(p => p.Profesion)
                .HasColumnName("Role code")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(p => p.Email)
                .HasColumnName("Communication identifier")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(p => p.CodigoRegente)
                .HasColumnName("Identifier")
                .HasMaxLength(20)
                .IsRequired();
            builder.Property(p => p.IdTipoIdentificacion)
                .IsRequired();
            builder.Property(p => p.Activo)
                .IsRequired()
                .HasDefaultValue(1);
           
        }
    }
}
