using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class ProductosConfiguration : IEntityTypeConfiguration<Productos>
    {
        public void Configure(EntityTypeBuilder<Productos> builder)
        {
            builder.ToTable("Productos");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Clase)
                .HasColumnName("Clase")
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(p => p.Presentacion)
                .HasColumnName("Presentacion")
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(p => p.NombreComun)
                .HasColumnName("NombreComun")
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(p => p.NombreCientifico)
                .HasColumnName("NombreCientifico")
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(p => p.Tradicional)
                .IsRequired();
        }
    }
}
