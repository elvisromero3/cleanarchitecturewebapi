using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class TipoAccionExcepcionMorosidadMappingProfile : Profile
    {
        public TipoAccionExcepcionMorosidadMappingProfile()
        {
            CreateMap<TipoAccionExcepcionMorosidad, TipoAccionExcepcionMorosidadDto>();
            CreateMap<TipoAccionExcepcionMorosidadDto, TipoAccionExcepcionMorosidad>();
        }
    }
}
