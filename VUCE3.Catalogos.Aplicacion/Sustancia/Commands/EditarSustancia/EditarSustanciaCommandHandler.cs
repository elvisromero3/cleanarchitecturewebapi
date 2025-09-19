using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EditarSustancia
{
    public class EditarSustanciaCommandHandler : IRequestHandler<EditarSustanciaCommand, ErrorOr<Tuple<Dominio.Entidades.Sustancia, Dominio.Entidades.Sustancia>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarSustanciaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.Sustancia, Dominio.Entidades.Sustancia>>> Handle(EditarSustanciaCommand command, CancellationToken cancellationToken)
        {
            var sustancia = await _unitOfWork.SustanciasRepository.ObtenerSustanciaPorId(command.IdSustancia);

            // Sustancia.NoEncontrada
            if (sustancia.IsError)
            {
                return sustancia.Errors;
            }

            var comprobarNombre = sustancia.Value.Nombre;
            var comprobarNumeroCas = sustancia.Value.Cas;
            var comprobarListaCaq = sustancia.Value.ListaCaq;
            var comprobarDuplicado = false;

            //se obtiene copia del sector antes de aplicar cambios
            Dominio.Entidades.Sustancia sustanciaAntes = JsonConvert.DeserializeObject<Dominio.Entidades.Sustancia>(
                JsonConvert.SerializeObject(sustancia.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? sustancia.Value;

            // SustanciaNombreInvalido
            if (command.ListaCambios.Contains("Nombre"))
            {
                comprobarNombre = command.Sustancia.Nombre;
                comprobarDuplicado = true;

                if (Validadores.LongitudMaximaNoNull(command.Sustancia.Nombre, 300))
                {
                    return ErroresSustancia.SustanciaNombreInvalido;
                }
            }
          
            // SustanciasCasInvalido
            if (command.ListaCambios.Contains("Cas"))
            {
                comprobarNumeroCas = command.Sustancia.Cas;
                comprobarDuplicado = true;

                if (Validadores.LongitudMaximaNoNull(command.Sustancia.Cas, 100))
                {
                    return ErroresSustancia.SustanciasCasInvalido;
                }
            }
          
            // SustanciaListaCaqInvalido
            if (command.ListaCambios.Contains("ListaCaq"))
            {
                comprobarListaCaq = command.Sustancia.ListaCaq;
                comprobarDuplicado = true;

                if (Validadores.LongitudMaximaNoNull(command.Sustancia.ListaCaq, 100))
                {
                    return ErroresSustancia.SustanciaListaCaqInvalido;
                }
            }

            if (comprobarDuplicado)
            {
                //se valida que los cambios no alteren la clave primaria
                var existe = await _unitOfWork.SustanciasRepository.ValidarSustancia(
                    command.IdSustancia,
                    comprobarNombre,
                    comprobarNumeroCas,
                    comprobarListaCaq);

                if (existe.Value)
                {
                    return ErroresSustancia.DatosDuplicados;
                }
            }

            var sustanciaResult = await _unitOfWork.SustanciasRepository.EditarSustancia(command.Sustancia, command.IdSustancia, command.ListaCambios);

            await _unitOfWork.Save();

            return Tuple.Create(sustanciaAntes, sustanciaResult.Value);
        }
    }
}
