using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Pais.Commands.EliminarPais
{
    public class EliminarPaisCommandHandler : IRequestHandler<EliminarPaisCommand, ErrorOr<Dominio.Entidades.Pais>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarPaisCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Pais>> Handle(EliminarPaisCommand command, CancellationToken cancellationToken)
        {
            var pais = await _unitOfWork.PaisesRepository.ObtenerPaisPorId(command.Id);

            if (pais.IsError)
            {
                return pais.Errors;
            }

            var result = await _unitOfWork.PaisesRepository.EliminarPais(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return pais;
        }
    }
}
