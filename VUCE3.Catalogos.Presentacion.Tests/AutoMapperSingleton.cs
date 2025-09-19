using AutoMapper;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class AutoMapperSingleton
    {
        private static IMapper _mapper = null!;

        public static IMapper Mapper
        {
            get
            {
                if (_mapper == null)
                {
                    /// Auto Mapper Configurations
                    var mappingConfig = new MapperConfiguration(mc =>
                    {
                        mc.AddMaps(typeof(Program));
                    });

                    IMapper mapper = mappingConfig.CreateMapper();
                    _mapper = mapper;
                }
                return _mapper;
            }
        }

    }
}
