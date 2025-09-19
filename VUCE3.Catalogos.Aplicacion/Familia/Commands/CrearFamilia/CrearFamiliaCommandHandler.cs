using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Familia.Commands.CrearFamilia
{
    public class CrearFamiliaCommandHandler : IRequestHandler<CrearFamiliaCommand, ErrorOr<Dominio.Entidades.Familia>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearFamiliaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Familia>> Handle(CrearFamiliaCommand request, CancellationToken cancellationToken)
        {
            //Verifica el tamaño de Nombre o si es vacío  
            if (Validadores.Nombre50(request.Familia.Nombre))
            {
                return ErroresFamilia.NombreInvalido;
            }

            var existe = await _unitOfWork.FamiliaRepository.ValidarFamilia(request.Familia.Id, request.Familia.Nombre);
            if (existe.Value)
            {
                return ErroresFamilia.DatosDuplicados;
            }

            var result = await _unitOfWork.FamiliaRepository.CrearFamilia(request.Familia);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }

    }
}
