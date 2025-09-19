using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EliminarPaisBloqueComercial
{
    public class EliminarPaisBloqueComercialCommandHandler : IRequestHandler<EliminarPaisBloqueComercialCommand, ErrorOr<Dominio.Entidades.PaisBloqueComercial>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarPaisBloqueComercialCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.PaisBloqueComercial>> Handle(EliminarPaisBloqueComercialCommand command, CancellationToken cancellationToken)
        {
            var paisBloqueComercial = await _unitOfWork.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(command.Id);

            if (paisBloqueComercial.IsError)
            {
                return paisBloqueComercial.Errors;
            }

            var result = await _unitOfWork.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return paisBloqueComercial;
        }
    }
}
