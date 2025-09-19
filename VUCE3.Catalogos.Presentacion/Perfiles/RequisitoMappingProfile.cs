using AutoMapper;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class RequisitoMappingProfile : Profile
    {
        public RequisitoMappingProfile()
        {
            CreateMap<Requisito, RequisitoDto>().ReverseMap();
            
            CreateMap<ImportarRequisitoDto, ImportarRequisitoCommandDto>()
           .ForMember(dest => dest.IdInstitucion, opt => opt.MapFrom(src => src.IdInstitucion))
           .ForMember(dest => dest.Pais, opt => opt.MapFrom(src => src.Pais))
           .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo))
           .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version))
           .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion));
        }
    }
}
