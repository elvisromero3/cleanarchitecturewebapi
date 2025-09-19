using AutoMapper;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class PaisBloqueComercialMappingProfile : Profile
    {
        public PaisBloqueComercialMappingProfile()
        {
            
            CreateMap<PaisBloqueComercial, PaisBloqueComercialDto>().ReverseMap();
            CreateMap<ImportarPaisBloqueComercialDto, PaisBloqueComercial>().ReverseMap();

            CreateMap<ImportarPaisBloqueComercialDto, ImportarPaisBloqueComercialCommandDto>()
          .ForMember(dest => dest.BloqueComercial, opt => opt.MapFrom(src => src.BloqueComercial))
          .ForMember(dest => dest.Pais, opt => opt.MapFrom(src => src.Pais));

        }
    }
}