using System;
using Newtonsoft.Json;

namespace Personas.Core.Dtos.TablasComunes
{
    public class GuardarInformacionAdicionalDto
    {
        public long codigoPersona { get; set; }
        public long codigoTabla { get; set; }
        public long codigoElemento { get; set; }
        public string observacion { get; set; }

        [JsonIgnore] public char estado { get; set; }
    }
}