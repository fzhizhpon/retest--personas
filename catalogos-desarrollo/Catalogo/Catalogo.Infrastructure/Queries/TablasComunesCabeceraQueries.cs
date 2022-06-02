namespace Catalogo.Infrastructure.Queries
{
    public class TablasComunesCabeceraQueries
    {


        public static string ObtenerTablasComunesCabecera(string esquema)
        {
            return "SELECT " + 
                   "SCTC.CODIGO_TABLA codigo," +
                   "SCTC.DESCRIPCION descripcion " +
                   $"FROM {esquema}.SIFV_CABECERA_TABLAS_COMUNES SCTC " +
                   " WHERE SCTC.ESTADO = 1 ORDER BY SCTC.ORDEN";
        }
        
    }
}