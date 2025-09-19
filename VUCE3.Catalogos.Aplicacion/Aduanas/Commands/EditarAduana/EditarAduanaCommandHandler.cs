using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EditarAduana
{
    public class EditarAduanaCommandHandler : IRequestHandler<EditarAduanaCommand, ErrorOr<Tuple<Aduana, Aduana>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarAduanaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Aduana, Aduana>>> Handle(EditarAduanaCommand command, CancellationToken cancellationToken)
        {
            //Verifica tamaño de Nombre o si es vacío 
            if (Validadores.Nombre(command.Aduana.Nombre))
            {
                return ErroresAduana.AduanaNombreInvalido;
            }

            var aduana = await _unitOfWork.AduanasRepository.ObtenerAduanaPorId(command.IdAduana);
            if (aduana.IsError)
            {
                return aduana.Errors;
            }

            var existe = await _unitOfWork.AduanasRepository.ValidarAduana(command.Aduana);
            if (existe.Value)
            {
                return ErroresAduana.DatosDuplicados;
            }            

            //se obtiene copia de la aduana antes de aplicar cambios
            Aduana aduanaAntes = JsonConvert.DeserializeObject<Aduana>(
                JsonConvert.SerializeObject(aduana.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? aduana.Value;

            var result = await _unitOfWork.AduanasRepository.ActualizarAduana(command.Aduana, command.IdAduana, command.ListaCambios);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(aduanaAntes, aduana.Value);
        }
    }
}
