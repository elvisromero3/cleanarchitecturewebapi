using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class SustanciaMappingProfile : Profile
    {
        public SustanciaMappingProfile()
        {
            CreateMap<Sustancia, SustanciaDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Cas, opt => opt.MapFrom(src => src.Cas))
                .ForMember(dest => dest.ListaCaq, opt => opt.MapFrom(src => src.ListaCaq));

            CreateMap<SustanciaDto, Sustancia>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Cas, opt => opt.MapFrom(src => src.Cas))
                .ForMember(dest => dest.ListaCaq, opt => opt.MapFrom(src => src.ListaCaq));
        }
    }
}
