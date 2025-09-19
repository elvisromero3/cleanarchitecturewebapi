using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.CodigoCliente).HasColumnName("Identifier").HasMaxLength(35);
            builder.Property(p => p.NombreCliente).HasColumnName("Master data party name").HasMaxLength(100);
            builder.Property(p => p.NumeroIdentificacion).HasColumnName("Master data party identifier").HasMaxLength(12);
            builder.Property(p => p.IdTipoIdentificacion).HasColumnName("IdentificationScheme.Identifier");
            builder.Property(p => p.FechaVencimiento).HasColumnName("Document expiration date/time");
        }
    }
}
