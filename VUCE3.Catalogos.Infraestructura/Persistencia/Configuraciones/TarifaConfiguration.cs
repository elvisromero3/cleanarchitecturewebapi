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
    public class TarifaConfiguration : IEntityTypeConfiguration<Tarifa>
    {
        public void Configure(EntityTypeBuilder<Tarifa> builder)
        {
            builder.ToTable("Tarifas");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.IdTarifa);
            builder.Property(p => p.IdTipoServicioPlataforma);
            builder.Property(p => p.IdInstitucion);
            builder.Property(p => p.TipoServicioPlataforma).IsRequired();
            builder.Property(p => p.IdServicioPlataforma);
            builder.Property(p => p.ServicioPlataforma);
            builder.Property(p => p.CodigoTarifa).IsRequired();
            builder.Property(p => p.DescripcionTarifa).IsRequired();
            builder.Property(p => p.Importe).HasPrecision(18, 2);
            builder.Property(p => p.IdMoneda);
            builder.Property(p => p.FechaInicio);
            builder.Property(p => p.FechaFin);
            builder.Property(p => p.TipoTarifa).IsRequired();

            builder.HasOne(p => p.Moneda)
                .WithMany(m => m.Tarifas)
                .HasForeignKey(p => p.IdMoneda)
                .IsRequired();
        }
    }
}
