using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.BloquesComerciales.Commands.EliminarBloquesComerciales
{
    public class EliminarBloquesComercialesCommandHandler : IRequestHandler<EliminarBloquesComercialesCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarBloquesComercialesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarBloquesComercialesCommand request, CancellationToken cancellationToken)
        {
            foreach (var idBloqueComercial in request.IdsBloquesComerciales)
            {
                var bloque = await _unitOfWork.BloqueComercialRepository.ObtenerBloqueComercialPorId(idBloqueComercial);
                if (bloque.IsError)
                {
                    return bloque.Errors;
                }                
            }
            foreach (var idBloqueComercial in request.IdsBloquesComerciales)
            {
                var result = await _unitOfWork.BloqueComercialRepository.EliminarBloqueComercial(idBloqueComercial);

                if (result.IsError)
                {
                    return result.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Deleted;
        }
    }
}
