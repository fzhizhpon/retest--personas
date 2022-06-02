using CoopCrea.Cross.Event.Src.Commands;
using Personas.Application.Messages.CommandHandlers;
using Personas.Core.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personas.Application.Messages.Commands
{
    public class AuditoriaCommand : Command
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

        public AuditoriaCommand() { }

        //public AuditoriaCommand(AuditoriaCommand auditoriaCommand)
        //{
        //    this.guid = auditoriaCommand.guid;
        //    this.codigoAgencia = auditoriaCommand.codigoAgencia;
        //    this.codigoSucursal = auditoriaCommand.codigoSucursal;
        //    this.codigoUsuario = auditoriaCommand.codigoUsuario;
        //    this.fechaRegistro = auditoriaCommand.fechaRegistro;
        //    this.ipPrivada = auditoriaCommand.ipPrivada;
        //    this.ipPublica = auditoriaCommand.ipPublica;
        //    this.modulo = auditoriaCommand.modulo;
        //    this.endPoint = auditoriaCommand.endPoint;
        //    this.entrada = auditoriaCommand.entrada;
        //    this.salida = auditoriaCommand.salida;
        //    this.navegador = auditoriaCommand.navegador;
        //}

    }
}
