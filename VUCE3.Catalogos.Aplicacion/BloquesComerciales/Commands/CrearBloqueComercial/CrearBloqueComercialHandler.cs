using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.CrearBloqueComercial
{
    public class CrearBloqueComercialCommandHandler : IRequestHandler<CrearBloqueComercialCommand, ErrorOr<Dominio.Entidades.BloqueComercial>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearBloqueComercialCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.BloqueComercial>> Handle(CrearBloqueComercialCommand request, CancellationToken cancellationToken)
        {
            if (Validadores.LongitudMaximaNoNull(request.BloqueComercial.Nombre, 300))
            {
                return ErroresBloqueComercial.NombreInvalido;
            }

            var existe = await _unitOfWork.BloqueComercialRepository.ValidarBloqueComercial(request.BloqueComercial.Id, request.BloqueComercial.Nombre);
            if (existe.Value)
            {
                return ErroresBloqueComercial.DatosDuplicados;
            }

            var result = await _unitOfWork.BloqueComercialRepository.CrearBloqueComercial(request.BloqueComercial);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
