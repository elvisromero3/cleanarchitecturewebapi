using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Variedad.Commands.EditarVariedad
{
    public class EditarVariedadCommandHandler : IRequestHandler<EditarVariedadCommand, ErrorOr<Tuple<Dominio.Entidades.Variedad, Dominio.Entidades.Variedad>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarVariedadCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.Variedad, Dominio.Entidades.Variedad>>> Handle(EditarVariedadCommand command, CancellationToken cancellationToken)
        {
            //Obtiene variedad por el identificador de variedad
            var variedad = await _unitOfWork.VariedadesRepository.ObtenerVariedadPorId(command.IdVariedad);
            if (variedad.IsError)
            {
                return variedad.Errors;
            }

            //Verifica tamaño del código
            if (command.ListaCambios.Contains("Codigo") && Validadores.Codigo17(command.Variedad.Codigo))
            {
                return ErroresVariedad.VariedadCodigoInvalido;
            }

            //Verifica tamaño del nombre
            if (command.ListaCambios.Contains("Nombre") && Validadores.Nombre(command.Variedad.Nombre))
            {
                return ErroresVariedad.VariedadNombreInvalido;
            }

            var comprobarCodigo = variedad.Value.Codigo;
            var comprobarNombre = variedad.Value.Nombre;
            var comprobarDuplicado = false;

            if (command.ListaCambios.Contains("Codigo"))
            {
                comprobarCodigo = command.Variedad.Codigo;
                comprobarDuplicado = true;
            }
            if (command.ListaCambios.Contains("Nombre"))
            {
                comprobarNombre = command.Variedad.Nombre;
                comprobarDuplicado = true;
            }
                        
            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.VariedadesRepository.ValidarVariedad(command.IdVariedad, comprobarCodigo, comprobarNombre);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresVariedad.DatosDuplicados;
                }
            }

            //se obtiene copia de variedad antes de aplicar cambios
            Dominio.Entidades.Variedad variedadAntes = JsonConvert.DeserializeObject<Dominio.Entidades.Variedad>(
                JsonConvert.SerializeObject(variedad.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? variedad.Value;

            var result = await _unitOfWork.VariedadesRepository.EditarVariedad(command.Variedad, command.IdVariedad, command.ListaCambios);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(variedadAntes, result.Value);
        }
    }
}
