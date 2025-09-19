using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class BarrioMappingProfile : Profile
    {
        public BarrioMappingProfile()
        {
            CreateMap<Barrio, BarrioDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.IdDistrito, opt => opt.MapFrom(src => src.IdDistrito))
                .ForMember(dest => dest.IdCanton, opt => opt.MapFrom(src => src.Distrito.IdCanton))
                .ForMember(dest => dest.IdProvincia, opt => opt.MapFrom(src => src.Distrito.Canton.IdProvincia));

            //Crear mapping de DTO a Entidad
            CreateMap<BarrioDto, Barrio>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo))
               .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
               .ForMember(dest => dest.IdDistrito, opt => opt.MapFrom(src => src.IdDistrito));
        }
    }
}
