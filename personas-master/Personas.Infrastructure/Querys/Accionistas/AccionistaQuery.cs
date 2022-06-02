namespace Personas.Infrastructure.Querys.Accionistas
{
    public static class AccionistaQuery
    {
        private static string tabla = "PERS_ACCIONISTAS";

        public static string obtenerTabla()
        {
            return tabla;
        }

        public static string GuardarAccionistas(string esquema)
        {
            return $@"INSERT INTO {esquema}.{tabla} (
	                    CODIGO_PERSONA, 
	                    CODIGO_ACCIONISTA, 
	                    PORCENTAJE_ACCIONES, 
	                    CODIGO_USUARIO_REGISTRA, 
	                    FECHA_USUARIO_REGISTRA) 
                    VALUES(
	                    :codigoPersona, 
	                    :codigoAccionista, 
	                    :porcentajeAcciones, 
	                    :codigoUsuarioRegistra, 
	                    :fechaUsuarioRegistra)";
        }

        public static string ActualizarAccionista(string esquema)
        {
            return $@"UPDATE {esquema}.{tabla} SET 
	                    PORCENTAJE_ACCIONES = :porcentajeAcciones, 
	                    CODIGO_USUARIO_REGISTRA = :codigoUsuarioRegistra, 
	                    FECHA_USUARIO_REGISTRA = :fechaUsuarioRegistra 
                    WHERE CODIGO_PERSONA = :codigoPersona 
                    AND CODIGO_ACCIONISTA = :codigoAccionista";
        }

        public static string ObtenerAccionistas(string esquema)
        {
            return $@"SELECT
	                    a.CODIGO_PERSONA codigoPersona,
	                    a.CODIGO_ACCIONISTA codigoAccionista,
	                    a.PORCENTAJE_ACCIONES porcentajeAcciones 
                    FROM {esquema}.{tabla} a";
        }
    }
}
