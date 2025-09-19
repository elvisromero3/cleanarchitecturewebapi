using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class EstablecimientoMappingProfile : Profile
    {
        public EstablecimientoMappingProfile()
        {
            CreateMap<Establecimiento, EstablecimientoDto>();
            CreateMap<EstablecimientoDto, Establecimiento>();
            CreateMap<ImportarEstablecimientoDto, Establecimiento>().ReverseMap();
        }
    }
}
