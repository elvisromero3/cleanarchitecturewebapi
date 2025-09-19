using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Provincia.Commands.CrearProvincia
{
    public class CrearProvinciaCommandHandler : IRequestHandler<CrearProvinciaCommand, ErrorOr<Dominio.Entidades.Provincia>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearProvinciaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Provincia>> Handle(CrearProvinciaCommand request, CancellationToken cancellationToken)
        {
            if (Validadores.Nombre50(request.Provincia.Nombre))
            {
                return ErroresProvincia.ProvinciaNombreInvalido;
            }

            if (Validadores.Codigo1(request.Provincia.Codigo))
            {
                return ErroresProvincia.ProvinciaCodigoInvalido;
            }

            var existe = await _unitOfWork.ProvinciaRepository.ValidarProvincia(request.Provincia.Id, request.Provincia.Codigo);
            if (existe.Value)
            {
                return ErroresProvincia.DatosDuplicados;
            }

            var result = await _unitOfWork.ProvinciaRepository.CrearProvincia(request.Provincia);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
