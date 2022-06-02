using Microsoft.AspNetCore.Mvc;
using Personas.Application.Middleware;
using Personas.Core.App;
using Personas.Core.Dtos.Accionistas;
using Personas.Core.Interfaces.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Personas.Api.Controllers
{
    [ServiceFilter(typeof(FiltroAuditoria))]
    [Route("api/[controller]")]
    [ApiController]
    public class AccionistasController : ControllerBase
    {
        private readonly IAccionistaService _service;

        public AccionistasController(IAccionistaService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene un conjunto de accionistas.
        /// </summary>
        /// <param name="accionista">Objeto: ObtenerAccionistas.</param>
        [HttpGet]
        public async Task<ActionResult<Respuesta>> ObtenerAccionistas([FromQuery] AccionistaRequest accionista)
        {

            return Ok(await _service.ObtenerAccionistas(accionista));

        }

        /// <summary>
        /// Guarda un conjunto de accionistas.
        /// </summary>
        /// <param name="accionistasDto">Objeto: GuardarAccionistas.</param>
        [HttpPost]
        public async Task<ActionResult<Respuesta>> GuardarAccionistas(List<GuardarAccionistaDto> accionistasDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _service.GuardarAccionistas(accionistasDto));

        }

        /// <summary>
        /// Actualiza un conjunto de accionistas.
        /// </summary>
        /// <param name="dto">Objeto: ActualizarAccionista.</param>
        [HttpPut]
        public async Task<ActionResult<Respuesta>> ActualizarAccionista(ActualizarAccionistaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _service.ActualizarAccionista(dto));

        }
    }
}
