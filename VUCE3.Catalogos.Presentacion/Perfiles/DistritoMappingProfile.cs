using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class DistritoMappingProfile : Profile
    {
        public DistritoMappingProfile()
        {            
            CreateMap<Distrito, DistritoDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.IdCanton, opt => opt.MapFrom(src => src.IdCanton))
                .ForMember(dest => dest.IdProvincia, opt => opt.MapFrom(src => src.Canton.IdProvincia));

            //Crear mapping de DTO a Entidad
            CreateMap<DistritoDto, Distrito>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo))
               .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
               .ForMember(dest => dest.IdCanton, opt => opt.MapFrom(src => src.IdCanton));               
        }
    }
}
