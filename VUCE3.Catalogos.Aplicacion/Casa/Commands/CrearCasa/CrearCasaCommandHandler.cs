using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Casa.Commands.CrearCasa
{
    public class CrearCasaCommandHandler : IRequestHandler<CrearCasaCommand, ErrorOr<Dominio.Entidades.Casa>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearCasaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Casa>> Handle(CrearCasaCommand request, CancellationToken cancellationToken)
        {
            //Verifica el tamaño de codigo o si es vacío 
            if (Validadores.Codigo17(request.Casa.Codigo))
            {
                return ErroresCasa.CasaCodigoInvalido;
            }

            //Verifica el tamaño de Nombre o si es vacío  
            if (Validadores.Nombre(request.Casa.Nombre))
            {
                return ErroresCasa.CasaNombreInvalido;
            }

            var existe = await _unitOfWork.CasaRepository.ValidarCasa(request.Casa.Id, request.Casa.Codigo, request.Casa.Nombre);
            if (existe.Value)
            {
                return ErroresCasa.DatosDuplicados;
            }

            var result = await _unitOfWork.CasaRepository.CrearCasa(request.Casa);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
