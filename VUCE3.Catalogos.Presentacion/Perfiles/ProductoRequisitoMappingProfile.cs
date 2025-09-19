using AutoMapper;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Migrations;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class ProductoRequisitoMappingProfile : Profile
    {
        public ProductoRequisitoMappingProfile()
        {
            
            CreateMap<ProductoRequisitoDto, ProductoRequisito>();
            CreateMap<ProductoRequisito, ProductoRequisitoDto>()
                .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => src.TipoProducto.IdCategoria));

            CreateMap<ImportarProductoRequisitoDto, ImportarProductoRequisitoCommandDto>()
            .ForMember(dest => dest.IdRequisito, opt => opt.MapFrom(src => src.IdRequisito))
            .ForMember(dest => dest.TipoProducto, opt => opt.MapFrom(src => src.TipoProducto))
            .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Categoria));
          


        }
    }
}