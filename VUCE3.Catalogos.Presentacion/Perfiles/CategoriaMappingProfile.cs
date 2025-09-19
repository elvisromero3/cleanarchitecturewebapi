using AutoMapper;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class CategoriaMappingProfile : Profile
    {
        public CategoriaMappingProfile()
        {
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<ImportarCategoriaDto, Categoria>().ReverseMap();

            CreateMap<ImportarCategoriaDto, ImportarCategoriaCommandDto>()
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.Institucion, opt => opt.MapFrom(src => src.Institucion));
        }
    }
}
