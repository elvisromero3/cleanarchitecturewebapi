using AutoMapper;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;

namespace VUCE3.Catalogos.Presentacion.Perfiles
{
    public class ImagenesMappingProfile : Profile
    {
        public ImagenesMappingProfile()
        {
            CreateMap<Imagenes, ImagenesDto>();
            CreateMap<ImagenesDto, Imagenes>();
        }
    }
}
