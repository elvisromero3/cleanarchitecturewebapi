using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.ToTable("Empresas");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Nombre)
                .HasColumnName("Master data party name")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(p => p.NumeroIdentificacion)
                .HasColumnName("Master data party identifier")
                .HasMaxLength(12)
                .IsRequired();
            builder.Property(e => e.IdTipoIdentificacion)
                .HasColumnName("IdentificationScheme.Identifier")
                .IsRequired();
            builder.HasOne(e => e.Profesional)
               .WithMany(e => e.Empresas)
               .HasForeignKey(e=>e.IdProfesional);
            
            builder.Navigation(i => i.Profesional).AutoInclude();
        }
    }
}
