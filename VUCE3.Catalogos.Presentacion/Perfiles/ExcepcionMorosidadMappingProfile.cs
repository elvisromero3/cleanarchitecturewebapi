using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class ExcepcionMorosidadMappingProfile : Profile
    {
        public ExcepcionMorosidadMappingProfile()
        {
            CreateMap<ExcepcionMorosidad, ExcepcionMorosidadDto>().ReverseMap();
            CreateMap<ExcepcionMorosidadDto, ExcepcionMorosidad>().ReverseMap();
        }
    }
}
