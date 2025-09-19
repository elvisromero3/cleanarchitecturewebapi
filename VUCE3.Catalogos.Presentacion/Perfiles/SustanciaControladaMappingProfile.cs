using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.ImportarDatos.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class SustanciaControladaMappingProfile : Profile
    {
        public SustanciaControladaMappingProfile()
        {
            
            CreateMap<SustanciaControlada, SustanciaControladaDto>().ReverseMap();
            CreateMap<ImportarSustanciaControladaDto, ImportarSustanciasControladasCommandDto>().ReverseMap();


        }
    }
}