﻿using System;
namespace Personas.Core.Dtos.CorreosElectronicos
{
	public class EliminarCorreoRequest
	{
        public int codigoPersona { get; set; }
        public int codigoCorreoElectronico { get; set; }
    }
}