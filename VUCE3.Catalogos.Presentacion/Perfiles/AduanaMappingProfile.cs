using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class AduanaMappingProfile : Profile
    {
        public AduanaMappingProfile()
        {            
            CreateMap<Aduana, AduanaDto>().ReverseMap();
            CreateMap<ImportarAduanaDto, Aduana>().ReverseMap();
        }
    }
}
