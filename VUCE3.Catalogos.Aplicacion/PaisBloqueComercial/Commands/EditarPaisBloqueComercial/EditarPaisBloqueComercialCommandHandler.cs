using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EditarPaisBloqueComercial;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EditarPaisBloqueComercial
{
    public class EditarPaisBloqueComercialCommandHandler : IRequestHandler<EditarPaisBloqueComercialCommand, ErrorOr<Tuple<Dominio.Entidades.PaisBloqueComercial, Dominio.Entidades.PaisBloqueComercial>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarPaisBloqueComercialCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.PaisBloqueComercial, Dominio.Entidades.PaisBloqueComercial>>> Handle(EditarPaisBloqueComercialCommand command, CancellationToken cancellationToken)
        {
            // Obtener PaisBloqueComercial por Id
            var paisBloqueComercial = await _unitOfWork.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(command.IdPaisBloqueComercial);
            if (paisBloqueComercial.IsError)
            {
                return paisBloqueComercial.Errors;
            }

            var comprobarIdBloqueComercial = paisBloqueComercial.Value.IdBloqueComercial;
            var comprobarIdPais = paisBloqueComercial.Value.IdPais;

            var comprobarDuplicado = false;

            if (command.ListaCambios.Contains("IdBloqueComercial") || command.ListaCambios.Contains("IdPais"))
            {
                comprobarIdBloqueComercial = command.PaisBloqueComercial.IdBloqueComercial;
                comprobarIdPais = command.PaisBloqueComercial.IdPais;

                comprobarDuplicado = true;
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.PaisBloqueComercialRepository.ValidarPaisBloqueComercial(comprobarIdBloqueComercial, comprobarIdPais);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresPaisBloqueComercial.DatosDuplicados;
                }
            }

            if (command.ListaCambios.Contains("IdBloqueComercial"))
            {
                var bloqueComercial = await _unitOfWork.BloqueComercialRepository.ObtenerBloqueComercialPorId(command.PaisBloqueComercial.IdBloqueComercial);
                if (bloqueComercial.IsError)
                {
                    return ErroresPaisBloqueComercial.BloqueComercialNoEncontrado;
                }
            }

            if(command.ListaCambios.Contains("IdPais"))
            {
                var pais = await _unitOfWork.PaisesRepository.ObtenerPaisPorId(command.PaisBloqueComercial.IdPais);
                if (pais.Value is null)
                {
                    return ErroresPaisBloqueComercial.PaisNoEncontrado;
                }
            }

            //se obtiene copia de paisBloqueComercial antes de aplicar cambios
            Dominio.Entidades.PaisBloqueComercial paisBloqueComercialAntes = JsonConvert.DeserializeObject<Dominio.Entidades.PaisBloqueComercial>(
                JsonConvert.SerializeObject(paisBloqueComercial.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? paisBloqueComercial.Value;

            var result = await _unitOfWork.PaisBloqueComercialRepository.ActualizarPaisBloqueComercial(command.IdPaisBloqueComercial, command.ListaCambios, command.PaisBloqueComercial);
            if (result.IsError)
            {
                return result.Errors;
            }
            await _unitOfWork.Save();

            return Tuple.Create(paisBloqueComercialAntes, paisBloqueComercial.Value);
        }
    }
}