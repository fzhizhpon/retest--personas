using System;
using Newtonsoft.Json;

namespace Personas.Core.Dtos.BienesMuebles
{
    public class GuardarBienesMueblesDto
    {
        public long codigoPersona { get; set; }
        public int tipoBienMueble { get; set; }
        public string codigoReferencia { get; set; }
        public string descripcion { get; set; }
        public float avaluoComercial { get; set; }


        public long numeroRegistro { get; set; }
        [JsonIgnore] public long codigoUsuarioActualiza { get; set; }
        [JsonIgnore] public DateTime fechaUsuarioActualiza { get; set; }
        [JsonIgnore] public char estado { get; set; }
    }
}