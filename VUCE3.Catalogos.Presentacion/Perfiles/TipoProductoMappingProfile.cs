using AutoMapper;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class TipoProductoMappingProfile : Profile
    {
        public TipoProductoMappingProfile()
        {
            
            CreateMap<TipoProducto, TipoProductoDto>().ReverseMap();

            CreateMap<ImportarTipoProductoDto, ImportarTipoProductoCommandDto>()
           .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Categoria))
           .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo))
           .ForMember(dest => dest.Institucion, opt => opt.MapFrom(src => src.Institucion));
        }
    }
}