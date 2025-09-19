using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EliminarNoticiasVuce
{
    public class EliminarNoticiasVuceCommandHandler : IRequestHandler<EliminarNoticiasVuceCommand, ErrorOr<Dominio.Entidades.NoticiasVuce>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarNoticiasVuceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.NoticiasVuce>> Handle(EliminarNoticiasVuceCommand command, CancellationToken cancellationToken)
        {
            var noticiaVuce = await _unitOfWork.NoticiasVuceRepository.ObtenerNoticiasVucePorId(command.Id);

            if (noticiaVuce.IsError)
            {
                return noticiaVuce.Errors;
            }

            var result = await _unitOfWork.NoticiasVuceRepository.EliminarNoticiasVuce(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return noticiaVuce;
        }
    }
}
