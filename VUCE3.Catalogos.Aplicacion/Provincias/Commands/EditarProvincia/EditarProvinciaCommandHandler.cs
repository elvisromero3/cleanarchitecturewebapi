using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Provincia.Commands.EditarProvincia
{
    public class EditarProvinciaCommandHandler : IRequestHandler<EditarProvinciaCommand, ErrorOr<Tuple<Dominio.Entidades.Provincia, Dominio.Entidades.Provincia>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarProvinciaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.Provincia, Dominio.Entidades.Provincia>>> Handle(EditarProvinciaCommand command, CancellationToken cancellationToken)
        {
            if (command.ListaCambios.Contains("Codigo") && Validadores.Codigo1(command.Provincia.Codigo))
            {
                return ErroresProvincia.ProvinciaCodigoInvalido;
            }

            if (command.ListaCambios.Contains("Nombre") && Validadores.Nombre50(command.Provincia.Nombre))
            {
                return ErroresProvincia.ProvinciaNombreInvalido;
            }

            // Obtener provincia por Id
            var provincia = await _unitOfWork.ProvinciaRepository.ObtenerProvinciaPorId(command.IdProvincia);
            if (provincia.IsError)
            {
                return provincia.Errors;
            }

            var comprobarCodigo = provincia.Value.Codigo;
            var comprobarDuplicado = false;

            if (command.ListaCambios.Contains("Codigo"))
            {
                comprobarCodigo = command.Provincia.Codigo;
                comprobarDuplicado = true;
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.ProvinciaRepository.ValidarProvincia(command.IdProvincia, comprobarCodigo);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresProvincia.DatosDuplicados;
                }
            }            

            //se obtiene copia de la provincia antes de aplicar cambios
            Dominio.Entidades.Provincia provinciaAntes = JsonConvert.DeserializeObject<Dominio.Entidades.Provincia>(
                JsonConvert.SerializeObject(provincia.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? provincia.Value;

            var result = await _unitOfWork.ProvinciaRepository.ActualizarProvincia(command.IdProvincia, command.ListaCambios, command.Provincia);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(provinciaAntes, provincia.Value);
        }
    }
}
