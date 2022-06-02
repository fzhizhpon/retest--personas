using Personas.Core.Entities;

namespace Personas.Core.Dtos.Accionistas
{
    public class GuardarAccionistaDto : Auditoria
    {
        public long codigoPersona { get; set; }

        public long codigoAccionista { get; set; }

        public decimal porcentajeAcciones { get; set; }
    }
}
