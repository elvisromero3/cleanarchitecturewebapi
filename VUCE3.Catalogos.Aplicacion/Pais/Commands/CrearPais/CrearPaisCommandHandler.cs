using ErrorOr;
using MediatR;
using System.Text.RegularExpressions;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Pais.Commands.CrearPais
{
    public partial class CrearPaisCommandHandler : IRequestHandler<CrearPaisCommand, ErrorOr<Dominio.Entidades.Pais>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearPaisCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Pais>> Handle(CrearPaisCommand request, CancellationToken cancellationToken)
        {           

            var existe = await _unitOfWork.PaisesRepository.ValidarPais(request.Pais.Id, request.Pais.Nombre, request.Pais.CodigoA2, request.Pais.CodigoNumerico, request.Pais.CodigoC3);            

            if (existe.Value is not null)
            {
                if (Validadores.NormalizarString(existe.Value.Nombre) == Validadores.NormalizarString(request.Pais.Nombre))
                {
                    return ErroresPaises.NombreDuplicado;
                }

                if (existe.Value.CodigoA2 == request.Pais.CodigoA2)
                {
                    return ErroresPaises.CodigoA2Duplicado;
                }

                if (existe.Value.CodigoC3 == request.Pais.CodigoC3)
                {
                    return ErroresPaises.CodigoC3Duplicado;
                }

                if (existe.Value.CodigoNumerico == request.Pais.CodigoNumerico)
                {
                    return ErroresPaises.CodigoNumericoDuplicado;
                }
            }

            if (Validadores.Nombre(request.Pais.Nombre))
            {
                return ErroresPaises.ValidacionNombre;
            }

            if (!Validadores.IsValidoCodigoA2(request.Pais.CodigoA2))
            {
                return ErroresPaises.CodigoA2ExcedeLimite;
            }
            if (!Validadores.IsValidoCodigoNumerico(request.Pais.CodigoNumerico))
            {
                return ErroresPaises.CodigoNumericoExcedeLimite;
            }
            if (!Validadores.IsValidoCodigoC3(request.Pais.CodigoC3))
            {
                return ErroresPaises.CodigoC3ExcedeLimite;
            }


            var result = await _unitOfWork.PaisesRepository.CrearPais(request.Pais);

            if (result.IsError)
            {
                return existe.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
