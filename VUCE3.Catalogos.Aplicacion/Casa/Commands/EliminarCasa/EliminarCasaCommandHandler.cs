using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Casa.Commands.EliminarCasa
{
    public class EliminarCasaCommandHandler : IRequestHandler<EliminarCasaCommand, ErrorOr<Dominio.Entidades.Casa>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarCasaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Casa>> Handle(EliminarCasaCommand command, CancellationToken cancellationToken)
        {
            var casa = await _unitOfWork.CasaRepository.ObtenerCasaPorId(command.Id);

            if (casa.IsError)
            {
                return casa.Errors;
            }

            var result = await _unitOfWork.CasaRepository.EliminarCasa(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return casa;
        }
    }
}

