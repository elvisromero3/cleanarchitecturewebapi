using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    public class RequisitosConfiguration : IEntityTypeConfiguration<Requisito>
    {
        public void Configure(EntityTypeBuilder<Requisito> builder)
        {
            builder.ToTable("Requisitos");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Codigo)
                .HasMaxLength(50);
            builder.Property(p => p.Descripcion)
                .HasMaxLength(300)
                .HasColumnName("Additional statement description");
            builder.Property(p => p.Version)
                .HasMaxLength(10)
                .HasColumnName("Additional statement description-V");
            builder.Property(p => p.ImagenRequisito)
                .HasColumnName("Binary file type code")
                .HasColumnType("varchar(max)");
            builder.Property(p => p.NombreImagenRequisito)
                .HasColumnType("varchar(max)");
            builder.Property(p => p.IdPais)
                .HasColumnName("Destination Country name");
            builder.Property(p => p.IdInstitucion)
                .HasColumnName("Responsible government agency identifier");

            builder.HasOne(c => c.Pais)
                .WithMany(p => p.Requisitos)
                .HasForeignKey(c => c.IdPais)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
