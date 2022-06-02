namespace Personas.Infrastructure.Querys.Personas
{
    public static class PersonaNoNaturalQuery
    {
		private static string tabla = "PERS_PERSONAS_NO_NATURALES";

		public static string obtenerTabla()
		{
			return tabla;
		}

		public static string GuardarPersonaNoNatural(string esquema)
        {
            return $"INSERT INTO {esquema}.{tabla} (" +
				"	CODIGO_PERSONA, " +
				"	RAZON_SOCIAL, " +
				"	FECHA_CONSTITUCION, " +
				"	OBJETO_SOCIAL, " +
				"	FINALIDAD_LUCRO, " +
				"	CODIGO_USUARIO_ACTUALIZA, " +
				"	TIPO_SOCIEDAD, " +
				"	OBLIGADO_LLEVAR_CONTABILIDAD, " +
				"	AGENTE_RETENCION, " +
				"	DIRECCION_WEB, " +
				"	FECHA_USUARIO_ACTUALIZA) " +
				" VALUES(" +
				"	:codigoPersona," +
				"	:razonSocial," +
				"	:fechaConstitucion," +
				"	:objetoSocial," +
				"	:finalidadLucro," +
				"	:codigoUsuarioActualiza," +
				"	:tipoSociedad," +
				"	:obligadoLlevarContabilidad," +
				"	:agenteRetencion," +
				"	:direccionWeb," +
				"	:fechaUsuarioActualiza)";
        }

		public static string ActualizarPersonaNoNatural(string esquema) 
		{
			return $"UPDATE {esquema}.{tabla} SET " +
				"	RAZON_SOCIAL = :razonSocial, " +
				"	FECHA_CONSTITUCION = :fechaConstitucion, " +
				"	OBJETO_SOCIAL = :objetoSocial, " +
				"	FINALIDAD_LUCRO = :finalidadLucro, " +
				"	TIPO_SOCIEDAD = :tipoSociedad, " +
				"	CODIGO_USUARIO_ACTUALIZA = :codigoUsuarioActualiza, " +
				"	FECHA_USUARIO_ACTUALIZA = :fechaUsuarioActualiza," +
				"	OBLIGADO_LLEVAR_CONTABILIDAD = :obligadoLlevarContabilidad," +
				"	AGENTE_RETENCION = :agenteRetencion," +
				"	DIRECCION_WEB = :direccionWeb" +
				" WHERE CODIGO_PERSONA = :codigoPersona";
		}

		public static string ObtenerPersonaNoNatural(string esquema)
        {
			return "SELECT " +
				"	CODIGO_PERSONA codigoPersona," +
				"	RAZON_SOCIAL razonSocial, " +
				"	FECHA_CONSTITUCION fechaConstitucion," +
				"	OBJETO_SOCIAL objetoSocial, " +
				"	FINALIDAD_LUCRO finalidadLucro," +
				"	TIPO_SOCIEDAD tipoSociedad," +
				"	OBLIGADO_LLEVAR_CONTABILIDAD AS obligadoLlevarContabilidad, " +
				"	AGENTE_RETENCION AS agenteRetencion, " +
				"	DIRECCION_WEB AS direccionWeb, " +
				"	CODIGO_USUARIO_ACTUALIZA codigoUsuarioActualiza," +
				"	FECHA_USUARIO_ACTUALIZA fechaUsuarioActualiza" +
				$" FROM {esquema}.{tabla} WHERE CODIGO_PERSONA = :codigoPersona";
        }

	}
}
