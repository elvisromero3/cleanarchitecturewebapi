using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class CaracteristicaTipoProductonfiguration : IEntityTypeConfiguration<CaracteristicaTipoProducto>
    {
        public void Configure(EntityTypeBuilder<CaracteristicaTipoProducto> builder)
        {
            builder.ToTable("CaracteristicaTipoProducto");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id);
            builder.HasOne(e => e.Caracteristica)
               .WithMany(e => e.CaracteristicaTipoProductos)
               .HasForeignKey("IdCaracteristica");

            builder.HasOne(e => e.TipoProducto)
               .WithMany(e => e.CaracteristicaTipoProductos)
               .HasForeignKey("IdTipoProducto");
        }
    }
}
