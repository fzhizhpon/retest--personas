using System;
using System.Text.Json.Serialization;

namespace Personas.Core.Dtos.BienesIntangibles
{
    public class ActualizarBienesIntangiblesDto
    {
        public long codigoPersona { get; set; }
        public long numeroRegistro { get; set; }
        //public int tipoBienIntangible { get; set; }
        //public string codigoReferencia { get; set; }
        public string descripcion { get; set; }
        public float avaluoComercial { get; set; }


        [JsonIgnore] public long codigoUsuarioActualiza { get; set; }
        [JsonIgnore] public DateTime fechaUsuarioActualiza { get; set; }
    }
}