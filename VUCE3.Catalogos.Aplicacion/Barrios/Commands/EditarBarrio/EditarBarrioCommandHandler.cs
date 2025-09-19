using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Commands.EditarBarrio
{
    public class EditarBarrioCommandHandler : IRequestHandler<EditarBarrioCommand, ErrorOr<Tuple<Barrio, Barrio>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarBarrioCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Barrio, Barrio>>> Handle(EditarBarrioCommand command, CancellationToken cancellationToken)
        {
            // Obtener barrio por Id
            var barrio = await _unitOfWork.BarriosRepository.ObtenerBarrioPorId(command.IdBarrio);
            if (barrio.IsError)
            {
                return barrio.Errors;
            }

            if (command.ListaCambios.Contains("IdDistrito"))
            {
                return ErroresBarrio.BarrioDistritoNoActualizable;
            }

            //Valida tamaño nombre
            if (command.ListaCambios.Contains("Nombre") && Validadores.Nombre100(command.Barrio.Nombre))
            {
                return ErroresBarrio.BarrioNombreInvalido;
            }
            //Valida tamaño código
            if (command.ListaCambios.Contains("Codigo") && Validadores.Codigo7(command.Barrio.Codigo))
            {
                return ErroresBarrio.BarrioCodigoInvalido;
            }

            var comprobarCodigo = barrio.Value.Codigo;
            var comprobarDuplicado = false;
            
            if (command.ListaCambios.Contains("Codigo"))
            {
                comprobarCodigo = command.Barrio.Codigo;
                comprobarDuplicado = true;
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.BarriosRepository.ValidarBarrio(command.IdBarrio, comprobarCodigo, 0);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresBarrio.BarrioDatosDuplicados;
                }
            }

            //se obtiene copia de la barrio antes de aplicar cambios
            Barrio barrioAntes = JsonConvert.DeserializeObject<Barrio>(
                JsonConvert.SerializeObject(barrio.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? barrio.Value;

            var result = await _unitOfWork.BarriosRepository.ActualizarBarrio(command.Barrio, command.IdBarrio, command.ListaCambios);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(barrioAntes, barrio.Value);
        }
    }
}
