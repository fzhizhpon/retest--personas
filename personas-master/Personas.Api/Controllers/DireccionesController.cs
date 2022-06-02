using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Personas.Application.Middleware;
using Personas.Core.Interfaces.IServices;
using Personas.Core.App;
using Personas.Core.Dtos.Direcciones;

namespace Personas.Api.Controllers
{
    [ServiceFilter(typeof(FiltroAuditoria))]
    [Route("api/[controller]")]
    [ApiController]
    public class DireccionesController : ControllerBase
    {
        protected readonly IDireccionesService _service;

        public DireccionesController(IDireccionesService service)
        {
            _service = service;
        }

        /// <summary>
        /// Permite guardar una direccion.
        /// </summary>
        /// <param name="dto">Objeto: GuardarDireccionDto.</param>
        [HttpPost]
        public async Task<ActionResult<Respuesta>> GuardarDireccion(GuardarDireccionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            Respuesta respuesta = await _service.GuardarDireccion(dto);

            return Ok(respuesta);
        }

        /// <summary>
        /// Permite eliminar una direcci�n dado el c�digo de direcci�n y persona.
        /// </summary>
        /// <param name="dto">Objeto: EliminarDireccionDto.</param>
        [HttpDelete]
        public async Task<ActionResult<Respuesta>> EliminarDireccion([FromQuery] EliminarDireccionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Respuesta respuesta = await _service.EliminarDireccion(dto);

            return Ok(respuesta);
        }

        /// <summary>
        /// Permite actualizar una direcci�n.
        /// </summary>
        /// <param name="dto">Objeto: ActualizarDireccionDto.</param>
        [HttpPut]
        public async Task<ActionResult<Respuesta>> ActualizarDireccion(ActualizarDireccionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Respuesta respuesta = await _service.ActualizarDireccion(dto);

            return Ok(respuesta);
        }

        /// <summary>
        /// Permite obtener una direccion dependiendo los par�metros codigoPersona y codigoDireccion.
        /// </summary>
        /// <param name="codigoPersona">C�digo de la persona.</param>
        /// <param name="numeroRegistro">C�digo de la direcci�n.</param>
        [HttpGet("{codigoPersona}/{numeroRegistro}")]
        public async Task<ActionResult<Respuesta>> ObtenerDireccion(int codigoPersona, int numeroRegistro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Respuesta respuesta = await _service.ObtenerDireccion(new ObtenerDireccionDto
            {
                codigoPersona = codigoPersona,
                numeroRegistro = numeroRegistro
            }); 

            return Ok(respuesta);
        }

        /// <summary>
        /// Permite obtener una lista de direcciones dependiendo los par�metros opcionales.
        /// </summary>
        /// <param name="dto">Objeto: ObtenerDireccionesDto.</param>
        [HttpGet]
        public async Task<ActionResult<Respuesta>> ObtenerDirecciones([FromQuery] ObtenerDireccionesDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Respuesta respuesta = await _service.ObtenerDirecciones(dto);

            return Ok(respuesta);
        }
    }
}