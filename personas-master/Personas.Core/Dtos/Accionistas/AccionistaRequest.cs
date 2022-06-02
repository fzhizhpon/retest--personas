using Vimasistem.QueryFilter.Attributes;

namespace Personas.Core.Dtos.Accionistas
{
    public class AccionistaRequest : PaginacionQueryDto
    {
        [Filter("CODIGO_PERSONA", "PERS_ACCIONISTAS")]
        public long codigoPersona { get; set; }

        [Filter("CODIGO_ACCIONISTA", "PERS_ACCIONISTAS")]
        public long? codigoAccionista { get; set; }
    }
}
