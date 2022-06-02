using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Personas.Application.Middleware;
using Personas.Core.App;
using Personas.Core.Dtos.TablasComunes;
using Personas.Core.Interfaces.IServices;

namespace Personas.Api.Controllers
{
    [ServiceFilter(typeof(FiltroAuditoria))]
    [Route("api/[controller]")]
    [ApiController]
    public class InformacionAdicionalController : ControllerBase
    {
        // * propiedad
        private readonly IInformacionAdicionalService _tablaComunesDetallesService;


        public InformacionAdicionalController(IInformacionAdicionalService tablaComunesDetallesService)
        {
            _tablaComunesDetallesService = tablaComunesDetallesService;
        }

        [HttpGet("{codigoPersona}/{codigoTabla}")]
        public async Task<ActionResult<Respuesta>> ObtenerInformacionAdicional(long codigoPersona, long codigoTabla)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _tablaComunesDetallesService.ObtenerInformacionAdicional(codigoPersona, codigoTabla));
        }

        [HttpPost]
        public async Task<ActionResult<Respuesta>> GuardarInformacionAdicional(GuardarInformacionAdicionalDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _tablaComunesDetallesService.GuardarInformacionAdicional(dto));
        }

        [HttpPut]
        public async Task<ActionResult<Respuesta>> ActualizarInformacionAdicional(ActualizarInformacionAdicionalDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _tablaComunesDetallesService.ActualizarInformacionAdicional(dto));
        }
    }
}