using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    public class BarrioConfiguration : IEntityTypeConfiguration<Barrio>
    {
        public void Configure(EntityTypeBuilder<Barrio> builder)
        {
            builder.ToTable("Barrios");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Codigo)
                .HasMaxLength(7)
                .HasColumnName("Street and number/P.O. Box-CB");
            builder.Property(p => p.Nombre)
                .HasMaxLength(100)
                .HasColumnName("Street and number/P.O. Box");
            builder.Property(p => p.IdDistrito)
                .HasColumnName("City name");

            builder.HasOne(c => c.Distrito)
                .WithMany(p => p.Barrios)
                .HasForeignKey(c => c.IdDistrito)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
