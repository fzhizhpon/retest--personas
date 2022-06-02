using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personas.Core.Dtos.App
{
    public class InformacionToken
    {
        public string codigoUsuario { get; set; }
        public string cedula { get; set; }
        public string usuarioAD { get; set; }
        public string codigoEmpresa { get; set; }
        public string codigoSucursal { get; set; }
        public string codigoAgencia { get; set; }
        public string rol { get; set; }
        public string codigoPeriodo { get; set; }
        public string navegador { get; set; }
        public string ipPublica { get; set; }
        public string ipPrivada { get; set; }
    }
}
