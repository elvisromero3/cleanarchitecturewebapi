using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;


namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.EliminarSustanciaControlada
{
    public class EliminarSustanciaControladaCommandHandler : IRequestHandler<EliminarSustanciaControladaCommand, ErrorOr<Dominio.Entidades.SustanciaControlada>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarSustanciaControladaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.SustanciaControlada>> Handle(EliminarSustanciaControladaCommand command, CancellationToken cancellationToken)
        {
            var provincia = await _unitOfWork.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(command.Id);

            if (provincia.IsError)
            {
                return provincia.Errors;
            }

            var result = await _unitOfWork.SustanciaControladaRepository.EliminarSustanciaControlada(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return provincia;
        }
    }
}

