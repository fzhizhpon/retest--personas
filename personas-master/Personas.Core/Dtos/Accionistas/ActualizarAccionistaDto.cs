using Personas.Core.Entities;

namespace Personas.Core.Dtos.Accionistas
{
    public class ActualizarAccionistaDto : Auditoria
    {
        public long codigoPersona { get; set; }

        public long codigoAccionista { get; set; }

        public decimal porcentajeAcciones { get; set; }
    }
}
