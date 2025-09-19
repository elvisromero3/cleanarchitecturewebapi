using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;


namespace VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.EliminarBloqueComercial
{
    public class EliminarBloqueComercialCommandHandler : IRequestHandler<EliminarBloqueComercialCommand, ErrorOr<Dominio.Entidades.BloqueComercial>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarBloqueComercialCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.BloqueComercial>> Handle(EliminarBloqueComercialCommand command, CancellationToken cancellationToken)
        {
            var bloqeucomercial = await _unitOfWork.BloqueComercialRepository.ObtenerBloqueComercialPorId(command.Id);

            if (bloqeucomercial.IsError)
            {
                return bloqeucomercial.Errors;
            }

            var result = await _unitOfWork.BloqueComercialRepository.EliminarBloqueComercial(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return bloqeucomercial;
        }
    }
}

