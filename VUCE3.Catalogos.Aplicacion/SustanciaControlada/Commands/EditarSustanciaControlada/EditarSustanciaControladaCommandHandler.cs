using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.EditarSustanciaControlada
{
    public class EditarSustanciaControladaCommandHandler : IRequestHandler<EditarSustanciaControladaCommand, ErrorOr<Tuple<Dominio.Entidades.SustanciaControlada, Dominio.Entidades.SustanciaControlada>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarSustanciaControladaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.SustanciaControlada, Dominio.Entidades.SustanciaControlada>>> Handle(EditarSustanciaControladaCommand command, CancellationToken cancellationToken)
        {
            // Obtener sustancia controlada por Id
            var sustanciacontrolada = await _unitOfWork.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(command.IdSustanciaControlada);
            if (sustanciacontrolada.IsError)
            {
                return sustanciacontrolada.Errors;
            }

            var comprobarClasificacionArancelaria = sustanciacontrolada.Value.ClasificacionArancelaria;
            var comprobarClasificacionAshrae = sustanciacontrolada.Value.ClasificacionAshrae;
            var comprobarPotencialCalentamientoGlobal = sustanciacontrolada.Value.PotencialCalentamientoGlobal;
            var comprobarTipoGas = sustanciacontrolada.Value.TipoGas;
            var comprobarIdFamilia = sustanciacontrolada.Value.IdFamilia;

            //se valida ClasificacionArancelaria
            if (command.ListaCambios.Contains("ClasificacionArancelaria"))
            {
                if (Validadores.LongitudMaximaNoNull(command.SustanciaControlada.ClasificacionArancelaria, 100))
                {
                    return ErroresSustanciaControlada.SustanciaControladaClasificacionArancelariaTamano;
                }

                comprobarClasificacionArancelaria = command.SustanciaControlada.ClasificacionArancelaria;
            }

            //se valida ClasificacionAshrae
            if (command.ListaCambios.Contains("ClasificacionAshrae"))
            {
                if (Validadores.LongitudMaximaNoNull(command.SustanciaControlada.ClasificacionAshrae, 100))
                {
                    return ErroresSustanciaControlada.SustanciaControladaClasificacionAshraeTamano;
                }

                comprobarClasificacionAshrae = command.SustanciaControlada.ClasificacionAshrae;
            }

            //se valida PotencialCalentamientoGlobal
            if (command.ListaCambios.Contains("PotencialCalentamientoGlobal")) 
            {
                if (Validadores.LongitudMaximaNoNull(command.SustanciaControlada.PotencialCalentamientoGlobal, 50))
                {
                    return ErroresSustanciaControlada.SustanciaControladaPotencialCalentamientoGlobalTamano;
                }

                comprobarPotencialCalentamientoGlobal = command.SustanciaControlada.PotencialCalentamientoGlobal;
            }

            //se valida TipoGas
            if (command.ListaCambios.Contains("TipoGas"))
            {
                if (Validadores.LongitudMaximaNoNull(command.SustanciaControlada.TipoGas, 100))
                {
                    return ErroresSustanciaControlada.SustanciaControladaTipoGasTamano;
                }

                comprobarTipoGas = command.SustanciaControlada.TipoGas;
            }
            
            if (command.ListaCambios.Contains("IdFamilia"))
            {
                var familia = await _unitOfWork.SustanciaControladaRepository.ObtenerFamiliaSustanciaControlada(command.SustanciaControlada.IdFamilia);
                if (familia.IsError)
                {
                    return familia.Errors;
                }

                comprobarIdFamilia = command.SustanciaControlada.IdFamilia;
            }

            //se obtiene copia de la sustancia controlada antes de aplicar cambios
            Dominio.Entidades.SustanciaControlada sustanciacontroladaAntes = JsonConvert.DeserializeObject<Dominio.Entidades.SustanciaControlada>(
                JsonConvert.SerializeObject(sustanciacontrolada.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
            ) ?? sustanciacontrolada.Value;

            var comprobarSustanciaControlada = new Dominio.Entidades.SustanciaControlada
            {
                ClasificacionArancelaria = comprobarClasificacionArancelaria,
                ClasificacionAshrae = comprobarClasificacionAshrae,
                PotencialCalentamientoGlobal = comprobarPotencialCalentamientoGlobal,
                IdFamilia = comprobarIdFamilia,
                TipoGas = comprobarTipoGas
            };
            var existe = await _unitOfWork.SustanciaControladaRepository.ValidarSustanciasControladas(command.IdSustanciaControlada, comprobarSustanciaControlada);
            if (existe.Value)
            {
                return ErroresSustanciaControlada.DatosDuplicados;
            }

            var result = await _unitOfWork.SustanciaControladaRepository.ActualizarSustanciaControlada(command.IdSustanciaControlada, command.ListaCambios, command.SustanciaControlada);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(sustanciacontroladaAntes, sustanciacontrolada.Value);
        }
    }
}
