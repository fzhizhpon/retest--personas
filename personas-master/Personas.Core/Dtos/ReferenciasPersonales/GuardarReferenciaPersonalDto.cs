using System;
using System.Text.Json.Serialization;

namespace Personas.Core.Dtos.ReferenciasPersonales
{
    public class GuardarReferenciaPersonalDto
    {
        public int codigoPersona { get; set; }
        public int numeroRegistro { get; set; }
        public int codigoPersonaReferida { get; set; }
        public DateTime? FechaConoce { get; set; }
        public string Observaciones { get; set; }


        [JsonIgnore]
        public char Estado { get; } = '1';
        [JsonIgnore]
        public string guid { get; set; }
        [JsonIgnore]
        public int codigoUsuarioActualiza { get; set; }
        [JsonIgnore]
        public DateTime? fechaUsuarioActualiza { get; set; }
    }
}
