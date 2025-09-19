using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EliminarSustancia
{
    public class EliminarSustanciaCommandHandler : IRequestHandler<EliminarSustanciaCommand, ErrorOr<Dominio.Entidades.Sustancia>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarSustanciaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Dominio.Entidades.Sustancia>> Handle(EliminarSustanciaCommand request, CancellationToken cancellationToken)
        {
            var sustancia = await _unitOfWork.SustanciasRepository.ObtenerSustanciaPorId(request.IdSustancia);

            if (sustancia.IsError)
            {
                return sustancia.Errors;
            }

            var result = await _unitOfWork.SustanciasRepository.EliminarSustancia(request.IdSustancia);

            if (result.IsError)
            {
                return result.Errors;
            }
            
            await _unitOfWork.Save();

            return sustancia;
        }
    }
}
