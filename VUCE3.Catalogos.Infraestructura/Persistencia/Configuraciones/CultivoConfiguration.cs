using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class CultivoConfiguration : IEntityTypeConfiguration<Cultivo>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Cultivo> builder)
        {
            builder.ToTable("Cultivos");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Codigo)
                .HasColumnName("Product identifier")
                .HasMaxLength(25)
                .IsRequired();
            builder.Property(e => e.Nombre)
                .HasColumnName("Commodity characteristics description")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(e => e.NombreCientifico)
                .HasColumnName("Commodity classification name code")
                .HasMaxLength(100)
                .IsRequired();
            builder.HasOne(e => e.Variedad)
                .WithMany(e => e.Cultivos)
                .HasForeignKey("IdVariedad")
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
            

            builder.Navigation(e => e.Variedad).AutoInclude();
        }
    }
}
