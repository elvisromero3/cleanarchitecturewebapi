using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class CultivoMappingProfile : Profile
    {
        public CultivoMappingProfile()
        {
            CreateMap<Cultivo, CultivoDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.NombreCientifico, opt => opt.MapFrom(src => src.NombreCientifico))
                .ForMember(dest => dest.IdVariedad, opt => opt.MapFrom(src => src.IdVariedad));

            CreateMap<CultivoDto, Cultivo>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.NombreCientifico, opt => opt.MapFrom(src => src.NombreCientifico))
                .ForMember(dest => dest.IdVariedad, opt => opt.MapFrom(src => src.IdVariedad));

            CreateMap<ImportarCultivoDto, Cultivo>()
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.NombreCientifico, opt => opt.MapFrom(src => src.NombreCientifico))
                .ForMember(dest => dest.Variedad, opt => opt.MapFrom(src => new Variedad { Codigo = src.CodigoVariedad }));
        }
    }
}
