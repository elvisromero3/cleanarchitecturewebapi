using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EliminarPaisBloquesComerciales
{
    public class EliminarPaisBloquesComercialesCommandHandler : IRequestHandler<EliminarPaisBloquesComercialesCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarPaisBloquesComercialesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarPaisBloquesComercialesCommand request, CancellationToken cancellationToken)
        {
            foreach (var idBloqueComercial in request.IdsBloquesComerciales)
            {
                var bloque = await _unitOfWork.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(idBloqueComercial);
                if (bloque.IsError)
                {
                    return bloque.Errors;
                }                
            }
            foreach (var idBloqueComercial in request.IdsBloquesComerciales)
            {
                var result = await _unitOfWork.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(idBloqueComercial);

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
