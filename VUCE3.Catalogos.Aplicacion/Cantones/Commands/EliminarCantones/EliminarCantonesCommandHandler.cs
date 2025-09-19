using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Cantones.Commands.EliminarCantones
{
    public class EliminarCantonesCommandHandler : IRequestHandler<EliminarCantonesCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarCantonesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarCantonesCommand request, CancellationToken cancellationToken)
        {
            var cantones = await _unitOfWork.CantonesRepository.ObtenerCantones();
            var cantonesDict = cantones.Value.ToDictionary(p => p.Id, p => p.Nombre);

            foreach (var idCanton in request.IdsCantones)
            {
                var canton = cantonesDict.Count(p => p.Key == idCanton);                
                if (canton==0)
                {
                    return ErroresCanton.NoEncontrado;
                }            
            }

            foreach (var idCanton in request.IdsCantones)
            {
                var result = await _unitOfWork.CantonesRepository.EliminarCantones(idCanton);

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
