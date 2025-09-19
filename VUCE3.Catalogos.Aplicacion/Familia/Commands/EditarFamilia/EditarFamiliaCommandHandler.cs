using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Familia.Commands.EditarFamilia
{
    public class EditarFamiliaCommandHandler : IRequestHandler<EditarFamiliaCommand, ErrorOr<Tuple<Dominio.Entidades.Familia, Dominio.Entidades.Familia>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarFamiliaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.Familia, Dominio.Entidades.Familia>>> Handle(EditarFamiliaCommand command, CancellationToken cancellationToken)
        {
            // Obtener Familia por Id
            var familia = await _unitOfWork.FamiliaRepository.ObtenerFamiliaPorId(command.IdFamilia);
            if (familia.IsError)
            {
                return familia.Errors;
            }

            //Verifica el tamaño de Nombre o si es vacío  
            if (command.ListaCambios.Contains("Nombre") && Validadores.Nombre50(command.Familia.Nombre))
            {
                return ErroresFamilia.NombreInvalido;
            }

            var comprobarNombre = familia.Value.Nombre;
            var comprobarDuplicado = false;

            if (command.ListaCambios.Contains("Nombre"))
            {
                comprobarNombre = command.Familia.Nombre;
                comprobarDuplicado = true;
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.FamiliaRepository.ValidarFamilia(command.IdFamilia, comprobarNombre);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresFamilia.DatosDuplicados;
                }
            }

            //se obtiene copia de la familia antes de aplicar cambios
            Dominio.Entidades.Familia familiaAntes = JsonConvert.DeserializeObject<Dominio.Entidades.Familia>(
                JsonConvert.SerializeObject(familia.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? familia.Value;

            var result = await _unitOfWork.FamiliaRepository.ActualizarFamilia(command.IdFamilia, command.ListaCambios, command.Familia);
            if (result.IsError)
            {
                return result.Errors;
            }
            await _unitOfWork.Save();

            return Tuple.Create(familiaAntes, familia.Value);
        }
    }
}