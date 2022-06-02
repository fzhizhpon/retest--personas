using System;
using System.Text.Json.Serialization;

namespace Personas.Core.Dtos.ReferenciasPersonales
{
    public class EliminarReferenciaPersonalDto
    {

        public int codigoReferenciaPersonal { get; set; }
        public int codigoPersonaReferida { get; set; }
        public int codigoPersona { get; set; }


        [JsonIgnore]
        public char estado { get; } = '1';
        [JsonIgnore]
        public int? codigoUsuarioActualiza { get; set; }
        [JsonIgnore]
        public DateTime? fechaUsuarioActualiza { get; set; }
        [JsonIgnore]
        public string guid { get; set; }
    }
}
