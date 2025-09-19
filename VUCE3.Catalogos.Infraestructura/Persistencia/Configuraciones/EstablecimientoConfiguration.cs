using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    public class EstablecimientoConfiguration : IEntityTypeConfiguration<Establecimiento>
    {
        public void Configure(EntityTypeBuilder<Establecimiento> builder)
        {
            builder.ToTable("Establecimientos");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.NumeroCvo).HasMaxLength(100);
            builder.Property(e => e.NombreEstablecimiento).HasMaxLength(100);
            builder.Property(e => e.ActividadPrimaria).HasMaxLength(100);
            builder.Property(e => e.ActividadSecundaria).HasMaxLength(100).IsRequired(false);
            builder.Property(e => e.Provincia).HasMaxLength(100);
            builder.Property(e => e.Canton).HasMaxLength(100);
            builder.Property(e => e.Distrito).HasMaxLength(100);
            builder.Property(e => e.DireccionExacta).HasMaxLength(250);
            builder.Property(e => e.FechaVencimiento);
            builder.Property(e => e.EstadoEstablecimiento).HasMaxLength(20);
          
        }
    }
}
