using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class CaracteristicaTipoProductoMappingProfile : Profile
    {
        public CaracteristicaTipoProductoMappingProfile()
        {
            CreateMap<CaracteristicaTipoProducto, CaracteristicaTipoProductoDto>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
             .ForMember(dest => dest.IdTipoProducto, opt => opt.MapFrom(src => src.IdTipoProducto))
             .ForMember(dest => dest.IdCaracteristica, opt => opt.MapFrom(src => src.IdCaracteristica))
             .ForMember(dest => dest.NombreCaracteristica, opt => opt.MapFrom(src => src.Caracteristica.Nombre));             
            
            CreateMap<CaracteristicaTipoProductoDto, CaracteristicaTipoProducto>();
        }
    }
}