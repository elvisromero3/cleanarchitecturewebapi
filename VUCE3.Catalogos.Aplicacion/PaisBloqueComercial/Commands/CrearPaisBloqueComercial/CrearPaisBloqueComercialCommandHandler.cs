using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.CrearPaisBloqueComercial
{
    public class CrearPaisBloqueComercialCommandHandler : IRequestHandler<CrearPaisBloqueComercialCommand, ErrorOr<Dominio.Entidades.PaisBloqueComercial>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearPaisBloqueComercialCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.PaisBloqueComercial>> Handle(CrearPaisBloqueComercialCommand request, CancellationToken cancellationToken)
        {
            var bloqueComercial = await _unitOfWork.BloqueComercialRepository.ObtenerBloqueComercialPorId(request.PaisBloqueComercial.IdBloqueComercial);
            if (bloqueComercial.Value is null)
            {
                return ErroresPaisBloqueComercial.BloqueComercialNoEncontrado;
            }

            var pais = await _unitOfWork.PaisesRepository.ObtenerPaisPorId(request.PaisBloqueComercial.IdPais);
            if (pais.Value is null)
            {
                return ErroresPaisBloqueComercial.PaisNoEncontrado;
            }

            var existe = await _unitOfWork.PaisBloqueComercialRepository.ValidarPaisBloqueComercial(request.PaisBloqueComercial.IdBloqueComercial, request.PaisBloqueComercial.IdPais);

            if (existe.Value)
            {
                return ErroresPaisBloqueComercial.DatosDuplicados;
            }

            var result = await _unitOfWork.PaisBloqueComercialRepository.CrearPaisBloqueComercial(request.PaisBloqueComercial);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
