using ErrorOr;
using MediatR;

using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Variedad.Commands.EliminarVariedad
{
    public class EliminarVariedadCommandHandler : IRequestHandler<EliminarVariedadCommand, ErrorOr<Dominio.Entidades.Variedad>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarVariedadCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Variedad>> Handle(EliminarVariedadCommand request, CancellationToken cancellationToken)
        {
            var variedad = await _unitOfWork.VariedadesRepository.ObtenerVariedadPorId(request.IdVariedad);

            if (variedad.IsError)
            {
                return variedad.Errors;
            }

            // Verifica si la intitucion tiene relacion en PerfilesUsuarios
            var existeRelacionCultivo = await _unitOfWork.VariedadesRepository.ExisteRelacionConCultivo(variedad.Value.Id);

            if (existeRelacionCultivo.IsError)
            {
                return existeRelacionCultivo.Errors;
            }

            if (existeRelacionCultivo.Value)
            {
                return ErroresVariedad.VariedadRelacionCultivo;
            }


            var result = await _unitOfWork.VariedadesRepository.EliminarVariedad(request.IdVariedad);

            if (result.IsError)
            {
                return result.Errors;
            }
            try
            {
                await _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) {
                    return Error.Conflict(description: ex.InnerException.Message);
                }
                return Error.Failure(description: ex.Message);
            }

            return variedad;
        }
    }
}
