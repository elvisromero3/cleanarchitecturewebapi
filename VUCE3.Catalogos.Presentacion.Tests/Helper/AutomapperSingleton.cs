using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace VUCE3.Catalogos.Presentacion.Tests.Helper
{
    public class AutomapperSingleton
    {
        private static IMapper? _mapper;

        public static IMapper Mapper
        {
            get
            {
                if (_mapper == null)
                {
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
