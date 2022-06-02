using Personas.Core.App;
using Personas.Core.Dtos.Accionistas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Personas.Core.Interfaces.IServices
{
    public interface IAccionistaService
    {
        Task<Respuesta> GuardarAccionistas(List<GuardarAccionistaDto> accionistasDto);
        
        Task<Respuesta> ActualizarAccionista(ActualizarAccionistaDto dto);
        
        Task<Respuesta> ObtenerAccionistas(AccionistaRequest dto);

    }
}
