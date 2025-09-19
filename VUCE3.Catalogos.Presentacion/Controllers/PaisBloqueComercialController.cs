using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Queries.ObtenerPaisBloqueComercial;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EditarPaisBloqueComercial;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Queries.ObtenerPaisBloqueComercialPorId;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.CrearPaisBloqueComercial;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EliminarPaisBloqueComercial;

using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EliminarPaisBloquesComerciales;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.Importardatos;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.ImportarDatos.DTO;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class PaisBloqueComercialController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public PaisBloqueComercialController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerPaisBloqueComercialQuery()
            {
            };

            var result = await _mediator.Send(query);

            return result.Match(
                        paisBloqueComercial => Ok(_mapper.Map<List<PaisBloqueComercialDto>>(paisBloqueComercial)),
                        error => Problem(error));
        }

        [EnableQuery]
        public async Task<IActionResult> Get(int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var query = new ObtenerPaisBloqueComercialPorIdQuery()
            {
                IdPaisBloqueComercial = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<PaisBloqueComercialDto>(result)),
              error => Problem(error));
        }

        public async Task<IActionResult> Post([FromBody] PaisBloqueComercialDto paisBloqueComercialDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearPaisBloqueComercialCommand()
            {
                PaisBloqueComercial = _mapper.Map<PaisBloqueComercial>(paisBloqueComercialDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        result => Created(_mapper.Map<PaisBloqueComercialDto>(result)),
                        error => Problem(error));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<PaisBloqueComercialDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            var resultDto = new PaisBloqueComercialDto();
            deltaDto.Patch(resultDto);
            var listaCambios = deltaDto.GetChangedPropertyNames().ToList();
            var result = _mapper.Map<PaisBloqueComercial>(resultDto);

            var command = new EditarPaisBloqueComercialCommand()
            {
                IdPaisBloqueComercial = key,
                PaisBloqueComercial = result,
                ListaCambios = listaCambios
            };

            var resultUpdate = await _mediator.Send(command);

            return resultUpdate.Match(
                updated => Ok(new Tuple<PaisBloqueComercialDto, PaisBloqueComercialDto>(_mapper.Map<PaisBloqueComercialDto>(updated.Item1),
                _mapper.Map<PaisBloqueComercialDto>(updated.Item2))),
                error => Problem(error));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarPaisBloqueComercialCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<PaisBloqueComercialDto>(deleted)),
                error => Problem(error));
        }

        [HttpPost]
        public async Task<IActionResult> BorradoMasivo(ODataActionParameters param)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var items = param["items"] as IEnumerable<int>;

            var command = new EliminarPaisBloquesComercialesCommand()
            {
                IdsBloquesComerciales = items ?? []
            };

            var res = await _mediator.Send(command);

            return res.Match(
                deleted => NoContent(),
                error => Problem(error));
        }

        [HttpPost]
        public async Task<IActionResult> ImportarDatos(ODataActionParameters param)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(y => y.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(error, false);
            }

            IEnumerable<ImportarPaisBloqueComercialDto> temp = (IEnumerable<ImportarPaisBloqueComercialDto>)param["datos"];

            var modo = (int)param["modo"];

            List<ImportarPaisBloqueComercialDto> datos;
            try
            {
                datos = temp.ToList();
            }
            catch (Exception)
            {
                return EstructuraArchivoImportacionInvalido();
            }

            var camposInvalidos = ValidadorDto.IsAnyNullOrEmpty(datos);

            if (camposInvalidos)
            {
                return EstructuraArchivoImportacionInvalido();
            }


            var command = new ImportarDatosCommand
            {
                Modo = modo,
                Datos = _mapper.Map<List<ImportarPaisBloqueComercialCommandDto>>(datos)
            };

            var result = await _mediator.Send(command);
                        
            return result.Match(
            deleted => Created(),
            error => Problem(error));
        }


    }
}
