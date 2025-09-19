using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Commands.EliminarBarrios
{
    public class EliminarBarriosCommandHandler : IRequestHandler<EliminarBarriosCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarBarriosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarBarriosCommand request, CancellationToken cancellationToken)
        {
            var barrios = await _unitOfWork.BarriosRepository.ObtenerBarrios();
            var barriosDict = barrios.Value.ToDictionary(p => p.Id, p => p.Nombre);

            foreach (var idBarrio in request.IdsBarrios)
            {
                var barrio = barriosDict.Count(p => p.Key == idBarrio);
                if (barrio == 0)
                {
                    return ErroresBarrio.BarrioNoEncontrado;
                }
            }

            foreach (var idBarrio in request.IdsBarrios)
            {
                var result = await _unitOfWork.BarriosRepository.EliminarBarrio(idBarrio);

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
