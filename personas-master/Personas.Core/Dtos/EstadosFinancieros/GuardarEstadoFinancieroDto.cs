using System;

namespace Personas.Core.Dtos.EstadosFinancieros
{
    public class GuardarEstadoFinancieroDto
    {
        public int codigoPersona { get; set; }
        public string cuentaContable { get; set; }
        public float valor { get; set; }
        public string observacion { get; set; }
        public DateTime fechaUsuarioActualiza { get; set; }
        public int codigoUsuarioActualiza { get; set; }


    }
}
