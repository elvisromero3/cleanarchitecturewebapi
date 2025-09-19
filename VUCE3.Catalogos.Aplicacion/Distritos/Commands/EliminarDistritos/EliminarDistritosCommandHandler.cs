using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Commands.EliminarDistritos
{
    public class EliminarDistritosCommandHandler : IRequestHandler<EliminarDistritosCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarDistritosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarDistritosCommand request, CancellationToken cancellationToken)
        {
            var distritos = await _unitOfWork.DistritosRepository.ObtenerDistritos();
            var distritosDict = distritos.Value.ToDictionary(p => p.Id, p => p.Nombre);

            foreach (var idDistrito in request.IdsDistritos)
            {
                var distrito = distritosDict.Count(p => p.Key == idDistrito);
                if (distrito == 0)
                {
                    return ErroresDistrito.DistritoNoEncontrado;
                }
            }

            foreach (var idDistrito in request.IdsDistritos)
            {
                var result = await _unitOfWork.DistritosRepository.EliminarDistrito(idDistrito);

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
