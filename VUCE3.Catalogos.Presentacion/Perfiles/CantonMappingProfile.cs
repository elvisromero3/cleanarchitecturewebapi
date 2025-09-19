using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class CantonMappingProfile : Profile
    {
        public CantonMappingProfile()
        {
            
            CreateMap<Canton, CantonDto>().ReverseMap();

         

        }
    }
}