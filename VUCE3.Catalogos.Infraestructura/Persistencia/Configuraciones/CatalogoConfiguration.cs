using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class CatalogoConfiguration : IEntityTypeConfiguration<Catalogo>
    {
        public void Configure(EntityTypeBuilder<Catalogo> builder)
        {
            builder.ToTable("Catalogos");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Alias)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.NombreNormalizado)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.VisibleEmpresa)
                .IsRequired();

            builder.Property(p => p.RequiereFirma)
                .IsRequired();

            builder.Property(p => p.AccionControlador)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.NombreIngles)
               .IsRequired()
               .HasMaxLength(50);
            
            builder.Property(p => p.NombreInglesNormalizado)
               .IsRequired()
               .HasMaxLength(50);

            builder.Property(p => p.IdFuncionalidad);
        }
    }
}
