using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;


namespace VUCE3.Catalogos.Aplicacion.Cantones.Commands.EliminarCanton
{
    public class EliminarCantonCommandHandler : IRequestHandler<EliminarCantonCommand, ErrorOr<Dominio.Entidades.Canton>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarCantonCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Canton>> Handle(EliminarCantonCommand command, CancellationToken cancellationToken)
        {
            var canton = await _unitOfWork.CantonesRepository.ObtenerCantonesPorId(command.Id);

            if (canton.IsError)
            {
                return canton.Errors;
            }

            var result = await _unitOfWork.CantonesRepository.EliminarCantones(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return canton;
        }
    }
}

