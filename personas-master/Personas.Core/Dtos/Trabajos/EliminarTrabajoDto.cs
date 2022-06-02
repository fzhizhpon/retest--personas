using System;

namespace Personas.Core.Dtos.Trabajos
{
    public class EliminarTrabajoDto
    {
        public int codigoTrabajo { get; set; }
        public int codigoPersona { get; set; }
        public int? codigoUsuarioRegistro { get; set; }
        public DateTime? fechaUsuarioRegistro { get; set; }
        public string guid { get; set; }
    }
}
