using Personas.Core.App;
using Personas.Core.Dtos.Representantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personas.Core.Interfaces.IServices
{
    public interface IRepresentantesService
    {
        Task<Respuesta> GuardarRepresentante(GuardarRepresentanteDto dto);
        Task<Respuesta> ActualizarRepresentante(ActualizarRepresentanteDto dto);
        Task<Respuesta> EliminarRepresentante(EliminarRepresentanteDto dto);
        Task<Respuesta> ObtenerRepresentante(int codigoPersona, int codigoRepresentante);
        Task<Respuesta> ObtenerRepresentantes(int codigoPersona);
    }
}
