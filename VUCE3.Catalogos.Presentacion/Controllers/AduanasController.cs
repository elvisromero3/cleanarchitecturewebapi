using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.CrearAduana;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EditarAduana;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EliminaAduana;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EliminarAduanas;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Aduanas.Queries.ObtenerAduanaPorId;
using VUCE3.Catalogos.Aplicacion.Aduanas.Queries.ObtenerAduanas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class AduanasController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;        

        public AduanasController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerAduanasQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                        casas => Ok(_mapper.Map<List<AduanaDto>>(casas)),
                        errors => Problem(errors));
        }

        [EnableQuery]        
        public async Task<IActionResult> Get(int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var query = new ObtenerAduanaPorIdQuery()
            {
                Id = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<AduanaDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] AduanaDto aduana)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearAduanaCommand()
            {
                Aduana = _mapper.Map<Aduana>(aduana)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        resultAduana => Created(_mapper.Map<AduanaDto>(resultAduana)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<AduanaDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var resultDTO = new AduanaDto();
            deltaDto.Patch(resultDTO);
            var aduana = _mapper.Map<Aduana>(resultDTO);
            var changeList = deltaDto.GetChangedPropertyNames().ToList();

            var command = new EditarAduanaCommand(aduana, key, changeList);

            var result = await _mediator.Send(command);

            return result.Match(
                updated => Ok(new Tuple<AduanaDto, AduanaDto>(_mapper.Map<AduanaDto>(updated.Item1),
                _mapper.Map<AduanaDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarAduanaCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<AduanaDto>(deleted)),
                errors => Problem(errors));
        }

        [HttpPost]
        public async Task<IActionResult> ImportarDatos(ODataActionParameters param)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var modo = (int)param["modo"];

            IEnumerable<ImportarAduanaDto> temp = (IEnumerable<ImportarAduanaDto>)param["datos"];
            
            List<ImportarAduanaDto> datos;
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

            var command = new ImportarDatosCommand()
            {
                Modo = modo,
                Datos = _mapper.Map<IEnumerable<Aduana>>(datos)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Created(),
                errors => Problem(errors));
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

            var command = new EliminarAduanasCommand()
            {
                IdsAduanas = items ?? []
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => NoContent(),
                errors => Problem(errors));
        }
    }
}
