namespace Personas.Core.Dtos.TablasComunes
{
    public class ActualizarInformacionAdicionalDto
    {
        public long codigoPersona { get; set; }

        public long codigoTabla { get; set; }

        public long codigoElemento { get; set; }

        public string observacion { get; set; }
        
        public char estado { get; set; }
    }
}