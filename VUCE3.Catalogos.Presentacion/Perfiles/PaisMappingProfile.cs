using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class PaisMappingProfile : Profile
    {
        public PaisMappingProfile()
        {
            CreateMap<Pais, PaisDto>().ReverseMap();
            CreateMap<ImportarPaisDto, Pais>();
        }
    }
}
