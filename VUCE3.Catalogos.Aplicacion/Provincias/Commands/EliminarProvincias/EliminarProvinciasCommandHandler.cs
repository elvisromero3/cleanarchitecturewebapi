using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Provincias.Commands.EliminarProvincias
{
    public class EliminarProvinciasCommandHandler : IRequestHandler<EliminarProvinciasCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarProvinciasCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarProvinciasCommand request, CancellationToken cancellationToken)
        {
            foreach (var idProvincia in request.IdsProvincias)
            {
                var provincia = await _unitOfWork.ProvinciaRepository.ObtenerProvinciaPorId(idProvincia);
                if (provincia.IsError)
                {
                    return provincia.Errors;
                }

                var cantones = await _unitOfWork.CantonesRepository.ObtenerCantones();
                var cantonesProvincia = cantones.Value.Count(x => x.IdProvincia == idProvincia);                
                if(cantonesProvincia > 0)
                {
                    return ErroresProvincia.ProvinciaCantonesRelacionados;
                }
            }

            foreach (var idProvincia in request.IdsProvincias)
            {
                var result = await _unitOfWork.ProvinciaRepository.EliminarProvincia(idProvincia);

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
