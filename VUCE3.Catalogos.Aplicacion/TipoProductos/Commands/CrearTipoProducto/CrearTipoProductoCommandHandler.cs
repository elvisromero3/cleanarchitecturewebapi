using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.CrearTipoProducto
{
    public class CrearTipoProductoCommandHandler : IRequestHandler<CrearTipoProductoCommand, ErrorOr<Dominio.Entidades.TipoProducto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearTipoProductoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.TipoProducto>> Handle(CrearTipoProductoCommand request, CancellationToken cancellationToken)
        {
           
            var existe = await _unitOfWork.TipoProductoRepository.ValidarTipoProducto(request.TipoProducto.Tipo, request.TipoProducto.IdCategoria, request.TipoProducto.IdInstitucion, request.TipoProducto.Id);
            if (existe.Value)
            {
                return ErroresTipoProducto.DatosDuplicados;
            }

            if (Validadores.LongitudMaximaNoNull(request.TipoProducto.Tipo, 200))
            {
                return ErroresTipoProducto.TipoTamano;
            }

            if(request.TipoProducto.IdInstitucion != ConstantesInstituciones.DCA && request.TipoProducto.IdInstitucion != ConstantesInstituciones.DIPOA)
            {
                return ErroresTipoProducto.InstitucionNoValida;
            }

            var result = await _unitOfWork.TipoProductoRepository.CrearTipoProducto(request.TipoProducto);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
