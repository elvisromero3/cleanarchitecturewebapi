using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;
namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class NoticiasVuceMappingProfile: Profile
    {
        public NoticiasVuceMappingProfile()
        {
            CreateMap<NoticiasVuce, NoticiasVuceDto>().ReverseMap();
            CreateMap<ImportarNoticiasVuceDto, NoticiasVuce>();
        }

    }
}
