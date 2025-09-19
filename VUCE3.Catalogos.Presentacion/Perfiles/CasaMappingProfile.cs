using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class CasaMappingProfile : Profile
    {
        public CasaMappingProfile()
        {
            
            CreateMap<Casa, CasaDto>().ReverseMap();

            CreateMap<ImportarCasaDto, Casa>().ReverseMap();            

        }
    }
}