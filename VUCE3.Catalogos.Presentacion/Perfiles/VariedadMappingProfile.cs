using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class VariedadMappingProfile : Profile
    {
        public VariedadMappingProfile()
        {
            CreateMap<Variedad, VariedadDto>().ReverseMap();
            CreateMap<ImportarVariedadDto, Variedad>();
        }
    }
}
