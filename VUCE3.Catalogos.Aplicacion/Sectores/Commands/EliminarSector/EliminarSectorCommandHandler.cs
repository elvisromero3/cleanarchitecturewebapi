using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Commands.EliminarSector
{
    public class EliminarSectorCommandHandler : IRequestHandler<EliminarSectorCommand, ErrorOr<Sector>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarSectorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Sector>> Handle(EliminarSectorCommand command, CancellationToken cancellationToken)
        {
            var sector = await _unitOfWork.SectoresRepository.ObtenerSectorPorId(command.Id);

            if (sector.IsError)
            {
                return sector.Errors;
            }

            var result = await _unitOfWork.SectoresRepository.EliminarSector(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return sector;
        }
    }
}
