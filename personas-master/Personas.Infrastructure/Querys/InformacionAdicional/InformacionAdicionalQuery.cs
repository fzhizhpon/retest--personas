namespace Personas.Infrastructure.Querys.TablasComunes
{
    public class InformacionAdicionalQuery
    {
        public static string ObtenerInformacionAdicional(string esquema)
        {
            return "SELECT " +
                   "SDTC.CODIGO_ELEMENTO codigoElemento, " +
                   "SDTC.DESCRIPCION_ELEMENTO descripcionElemento, " +
                   "pd.OBSERVACION observacion, " +
                   "COALESCE(pd.ESTADO, '0') estado " +
                   $"FROM {esquema}.SIFV_DETALLE_TABLAS_COMUNES SDTC " +
                   $"LEFT JOIN {esquema}.PERS_DETALLE_INF_ADIC pd " +
                   "ON SDTC.CODIGO_TABLA = pd.CODIGO_TABLA AND " +
                   "pd.CODIGO_PERSONA = :codigoPersona AND " +
                   "pd.CODIGO_ELEMENTO = SDTC.CODIGO_ELEMENTO " +
                   "WHERE SDTC.CODIGO_TABLA = :codigoTabla " +
                   "ORDER BY SDTC.ORDEN ";
        }

        public static string GuardarInformacionAdicional(string esquema)
        {
            return "INSERT INTO " +
                   $"{esquema}.PERS_DETALLE_INF_ADIC ( " +
                   "CODIGO_PERSONA, " +
                   "CODIGO_TABLA, " +
                   "CODIGO_ELEMENTO, " +
                   "OBSERVACION," +
                   "ESTADO ) VALUES (" +
                   ":codigoPersona, " +
                   ":codigoTabla, " +
                   ":codigoElemento," +
                   ":observacion," +
                   ":estado)";
        }

        public static string ActualizarInformacionAdicional(string esquema)
        {
            return $"update {esquema}.PERS_DETALLE_INF_ADIC " +
                   "SET OBSERVACION = :observacion, " +
                   "ESTADO  = :estado " +
                   "WHERE CODIGO_PERSONA = :codigoPersona and CODIGO_TABLA = :codigoTabla and CODIGO_ELEMENTO = :codigoElemento";
        }
    }
}