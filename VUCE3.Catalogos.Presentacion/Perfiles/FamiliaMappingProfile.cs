using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class FamiliaMappingProfile : Profile
    {
        public FamiliaMappingProfile()
        {
            
            CreateMap<Familia, FamiliaDto>().ReverseMap();
            CreateMap<ImportarFamiliaDto, Familia>().ReverseMap();

        }
    }
}