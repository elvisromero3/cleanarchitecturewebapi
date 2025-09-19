using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class ProfesionalMappingProfile : Profile
    {
        public ProfesionalMappingProfile()
        {
            CreateMap<Profesional, ProfesionalDto>();
            CreateMap<ProfesionalDto, Profesional>();
        }
    }
}
