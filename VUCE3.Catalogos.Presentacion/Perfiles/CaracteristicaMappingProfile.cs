using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class CaracteristicaMappingProfile : Profile
    {
        public CaracteristicaMappingProfile()
        {
            CreateMap<Caracteristica, CaracteristicaDto>().ReverseMap();            
        }
    }
}
