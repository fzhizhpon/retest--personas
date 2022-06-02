using CoopCrea.Cross.Event.Src.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personas.Application.Messages.Events
{
    public class AuditoriaEvent : Event
    {
        public string guid { get; set; }
        public int codigoUsuario { get; set; }
        public DateTime fechaRegistro { get; set; }
        public int codigoSucursal { get; set; }
        public int codigoAgencia { get; set; }
        public string modulo { get; set; }
        public string endPoint { get; set; }
        public string entrada { get; set; }
        public string salida { get; set; }
        public string ipPublica { get; set; }
        public string ipPrivada { get; set; }
        public string navegador { get; set; }

        public AuditoriaEvent() { }

        public AuditoriaEvent(AuditoriaEvent auditoriaEvent)
        {
            this.guid = auditoriaEvent.guid;
            this.codigoAgencia = auditoriaEvent.codigoAgencia;
            this.codigoSucursal = auditoriaEvent.codigoSucursal;
            this.codigoUsuario = auditoriaEvent.codigoUsuario;
            this.fechaRegistro = auditoriaEvent.fechaRegistro;
            this.ipPrivada = auditoriaEvent.ipPrivada;
            this.ipPublica = auditoriaEvent.ipPublica;
            this.modulo = auditoriaEvent.modulo;
            this.endPoint = auditoriaEvent.endPoint;
            this.entrada = auditoriaEvent.entrada;
            this.salida = auditoriaEvent.salida;
            this.navegador = auditoriaEvent.navegador;
        }

    }
}
