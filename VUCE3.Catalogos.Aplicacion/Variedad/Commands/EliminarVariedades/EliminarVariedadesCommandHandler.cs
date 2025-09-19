using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Variedad.Commands.EliminarVariedades
{
    public class EliminarVariedadesCommandHandler : IRequestHandler<EliminarVariedadesCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarVariedadesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarVariedadesCommand request, CancellationToken cancellationToken)
        {
            foreach(var idVariedad in request.IdsVariedades)
            {
                var existeRelacionCultivo = await _unitOfWork.VariedadesRepository.ExisteRelacionConCultivo(idVariedad);

                if (existeRelacionCultivo.IsError)
                {
                    return existeRelacionCultivo.Errors;
                }

                if (existeRelacionCultivo.Value)
                {
                    return ErroresVariedad.VariedadesRelacionCultivo;
                }


                var resultDelete = await _unitOfWork.VariedadesRepository.EliminarVariedad(idVariedad);

                if (resultDelete.IsError)
                {
                    return resultDelete.Errors;
                }
            }
        
           await _unitOfWork.Save();
           
           
            return Result.Deleted;
        }
    }
}
