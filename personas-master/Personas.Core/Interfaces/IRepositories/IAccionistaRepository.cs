using Personas.Core.Dtos.Accionistas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Personas.Core.Interfaces.IRepositories
{
    public interface IAccionistaRepository
    {
  
        Task<int> GuardarAccionistas(List<GuardarAccionistaDto> accionistasDto);
        
        Task<int> ActualizarAccionista(ActualizarAccionistaDto dto);

        Task<IEnumerable<AccionistaResponse>> ObtenerAccionistas(AccionistaRequest dto);
    }
}
