using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EliminarCultivo
{
    public class EliminarCultivoCommandHandler : IRequestHandler<EliminarCultivoCommand, ErrorOr<Dominio.Entidades.Cultivo>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarCultivoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Cultivo>> Handle(EliminarCultivoCommand request, CancellationToken cancellationToken)
        {
            var cultivo = await _unitOfWork.CultivosRepository.ObtenerCultivoPorId(request.IdCultivo);

            if (cultivo.IsError)
            {
                return cultivo.Errors;
            }

            var result = await _unitOfWork.CultivosRepository.EliminarCultivo(request.IdCultivo);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return cultivo;
        }
    }
}
