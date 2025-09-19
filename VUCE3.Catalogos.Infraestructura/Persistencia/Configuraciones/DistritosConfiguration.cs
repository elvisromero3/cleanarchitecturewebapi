using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    public class DistritosConfiguration : IEntityTypeConfiguration<Distrito>
    {
        public void Configure(EntityTypeBuilder<Distrito> builder)
        {
            builder.ToTable("Distritos");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Codigo)
                .HasMaxLength(5)
                .HasColumnName("City name-CD");
            builder.Property(p => p.Nombre)
                .HasMaxLength(50)
                .HasColumnName("City name");
            builder.Property(p => p.IdCanton)
                .HasColumnName("Country sub-entity identification code");

            builder.HasOne(c => c.Canton)
                .WithMany(p => p.Distritos)
                .HasForeignKey(c => c.IdCanton)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
