using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Cantones.Commands.CrearCanton
{
    public class CrearCantonesCommandHandler : IRequestHandler<CrearCantonCommand, ErrorOr<Dominio.Entidades.Canton>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearCantonesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Canton>> Handle(CrearCantonCommand request, CancellationToken cancellationToken)
        {
            if (Validadores.Nombre50(request.Canton.Nombre))
            {
                return ErroresCanton.NombreInvalido;
            }

            if (Validadores.Codigo3(request.Canton.Codigo))
            {
                return ErroresCanton.CantonCodigoInvalido;
            }

            var existe = await _unitOfWork.CantonesRepository.ValidarCantones(request.Canton.Id, request.Canton.Codigo, 0);
            if (existe.Value)
            {
                return ErroresCanton.DatosDuplicados;
            }

            var existeProvincia = await _unitOfWork.ProvinciaRepository.ObtenerProvinciaPorId(request.Canton.IdProvincia);
            if (existeProvincia.Value == null)
            {
                return ErroresCanton.ProvinciaInexistente;
            }

            var result = await _unitOfWork.CantonesRepository.CrearCantones(request.Canton);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
