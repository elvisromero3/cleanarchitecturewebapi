using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Pais.Commands.EditarPais
{
    public partial class EditarPaisCommandHandler : IRequestHandler<EditarPaisCommand, ErrorOr<Tuple<Dominio.Entidades.Pais, Dominio.Entidades.Pais>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarPaisCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.Pais, Dominio.Entidades.Pais>>> Handle(EditarPaisCommand command, CancellationToken cancellationToken)
        {
            var pais = await _unitOfWork.PaisesRepository.ObtenerPaisPorId(command.IdPais);
            if (pais.IsError)
            {
                return pais.Errors;
            }

            var comprobarNombre = pais.Value.Nombre;
            var comprobarCodigoA2 = pais.Value.CodigoA2;
            var comprobarCodigoNumerico = pais.Value.CodigoNumerico;
            var comprobarCodigoC3 = pais.Value.CodigoC3;            

            if (command.ListaCambios.Contains("Nombre"))
            {
                if (Validadores.Nombre(command.Pais.Nombre))
                {
                    return ErroresPaises.ValidacionNombre;
                }
                comprobarNombre = command.Pais.Nombre;                
            }
            if (command.ListaCambios.Contains("CodigoA2"))
            {
                if (!Validadores.IsValidoCodigoA2(command.Pais.CodigoA2))
                {
                    return ErroresPaises.CodigoA2ExcedeLimite;
                }
                comprobarCodigoA2 = command.Pais.CodigoA2;                
            }
            if (command.ListaCambios.Contains("CodigoNumerico"))
            {
                if (!Validadores.IsValidoCodigoNumerico(command.Pais.CodigoNumerico))
                {
                    return ErroresPaises.CodigoNumericoExcedeLimite;
                }
                comprobarCodigoNumerico = command.Pais.CodigoNumerico;                
            }
            if (command.ListaCambios.Contains("CodigoC3"))
            {
                if (!Validadores.IsValidoCodigoC3(command.Pais.CodigoC3))
                {
                    return ErroresPaises.CodigoC3ExcedeLimite;
                }
                comprobarCodigoC3 = command.Pais.CodigoC3;                
            }

            var existe = await _unitOfWork.PaisesRepository.ValidarPais(command.IdPais, comprobarNombre, comprobarCodigoA2, comprobarCodigoNumerico, comprobarCodigoC3);

            if (existe.IsError)
            {
                return existe.Errors;
            }

            if (existe.Value is not null)
            {
                if (Validadores.NormalizarString(existe.Value.Nombre) == Validadores.NormalizarString(command.Pais.Nombre))
                {
                    return ErroresPaises.NombreDuplicado;
                }

                if (existe.Value.CodigoA2 == command.Pais.CodigoA2)
                {
                    return ErroresPaises.CodigoA2Duplicado;
                }

                if (existe.Value.CodigoC3 == command.Pais.CodigoC3)
                {
                    return ErroresPaises.CodigoC3Duplicado;
                }

                if (existe.Value.CodigoNumerico == command.Pais.CodigoNumerico)
                {
                    return ErroresPaises.CodigoNumericoDuplicado;
                }
            }

            //se obtiene copia de pais antes de aplicar cambios
            Dominio.Entidades.Pais paisAntes = JsonConvert.DeserializeObject<Dominio.Entidades.Pais>(
                JsonConvert.SerializeObject(pais.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? pais.Value;

            var result = await _unitOfWork.PaisesRepository.ActualizarPais(command.Pais, command.IdPais, command.ListaCambios);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(paisAntes, pais.Value);
        }
    }
}
