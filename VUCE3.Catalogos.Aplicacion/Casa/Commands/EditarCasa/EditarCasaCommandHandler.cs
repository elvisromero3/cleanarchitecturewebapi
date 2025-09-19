using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Casa.Commands.EditarCasa
{
    public class EditarCasaCommandHandler : IRequestHandler<EditarCasaCommand, ErrorOr<Tuple<Dominio.Entidades.Casa, Dominio.Entidades.Casa>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarCasaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.Casa, Dominio.Entidades.Casa>>> Handle(EditarCasaCommand command, CancellationToken cancellationToken)
        {
            // Obtener casa por Id
            var casa = await _unitOfWork.CasaRepository.ObtenerCasaPorId(command.IdCasa);
            if (casa.IsError)
            {
                return casa.Errors;
            }

            //Verifica el tamaño de codigo o si es vacío 
            if (command.ListaCambios.Contains("Codigo") && Validadores.Codigo17(command.Casa.Codigo))
            {
                return ErroresCasa.CasaCodigoInvalido;
            }

            //Verifica el tamaño de Nombre o si es vacío  
            if (command.ListaCambios.Contains("Nombre") && Validadores.Nombre(command.Casa.Nombre))
            {
                    return ErroresCasa.CasaNombreInvalido;
            }

            var comprobarCodigo = casa.Value.Codigo;
            var comprobarNombre = casa.Value.Nombre;
            var comprobarDuplicado = false;

            if (command.ListaCambios.Contains("Codigo"))
            {
                comprobarCodigo = command.Casa.Codigo;
                comprobarDuplicado = true;
            }
            if (command.ListaCambios.Contains("Nombre"))
            {
                comprobarNombre = command.Casa.Nombre;
                comprobarDuplicado = true;
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.CasaRepository.ValidarCasa(command.IdCasa, comprobarCodigo, comprobarNombre);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresCasa.DatosDuplicados;
                }
            }            

            //se obtiene copia de la casa antes de aplicar cambios
            Dominio.Entidades.Casa casaAntes = JsonConvert.DeserializeObject<Dominio.Entidades.Casa>(
                JsonConvert.SerializeObject(casa.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? casa.Value;

            var result = await _unitOfWork.CasaRepository.ActualizarCasa(command.IdCasa, command.ListaCambios, command.Casa);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(casaAntes, casa.Value);
        }
    }
}
