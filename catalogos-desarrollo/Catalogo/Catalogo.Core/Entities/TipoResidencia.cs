﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogo.Core.Entities
{
    public class TipoResidencia
    {
        public int codigo { get; set; }
        public string descripcion { get; set; }
        public char estado { get; set; }
        public int orden { get; set; }
        public string codigoAlterno { get; set; }
    }
}
