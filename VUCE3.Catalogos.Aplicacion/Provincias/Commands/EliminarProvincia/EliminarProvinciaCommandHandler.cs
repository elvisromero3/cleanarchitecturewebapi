using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;


namespace VUCE3.Catalogos.Aplicacion.Provincia.Commands.EliminarProvincia
{
    public class EliminarProvinciaCommandHandler : IRequestHandler<EliminarProvinciaCommand, ErrorOr<Dominio.Entidades.Provincia>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarProvinciaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Provincia>> Handle(EliminarProvinciaCommand command, CancellationToken cancellationToken)
        {
            var provincia = await _unitOfWork.ProvinciaRepository.ObtenerProvinciaPorId(command.Id);
            if (provincia.IsError)
            {
                return provincia.Errors;
            }

            var cantones = await _unitOfWork.CantonesRepository.ObtenerCantones();
            var cantonesPorProvincia = cantones.Value.Count(x => x.IdProvincia == command.Id);            
            if (cantonesPorProvincia > 0)
            {
                return ErroresProvincia.ProvinciaCantonesRelacionados;
            }

            var result = await _unitOfWork.ProvinciaRepository.EliminarProvincia(command.Id);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return provincia;
        }
    }
}

