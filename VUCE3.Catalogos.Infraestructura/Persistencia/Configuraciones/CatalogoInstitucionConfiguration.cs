using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Configuraciones
{
    [ExcludeFromCodeCoverage]
    public class CatalogoInstitucionConfiguration : IEntityTypeConfiguration<CatalogoInstitucion>
    {
        public void Configure(EntityTypeBuilder<CatalogoInstitucion> builder)
        {
            builder.ToTable("CatalogosInstituciones");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.CatalogoId)
                .IsRequired();

            builder.Property(p => p.InstitucionId)
                .IsRequired();
        }
    }
}
