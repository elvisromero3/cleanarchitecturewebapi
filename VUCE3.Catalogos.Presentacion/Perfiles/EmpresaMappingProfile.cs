using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class EmpresaMappingProfile : Profile
    {
        public EmpresaMappingProfile()
        {
            CreateMap<Empresa, EmpresaDto>();

            CreateMap<EmpresaDto, Empresa>();
        }
    }
}
