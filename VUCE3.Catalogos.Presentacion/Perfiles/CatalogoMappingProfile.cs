using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class CatalogoMappingProfile : Profile
    {
        public CatalogoMappingProfile()
        {
            CreateMap<Catalogo, CatalogoDto>();
            CreateMap<CatalogoDto, Catalogo>();
        }
    }
}
