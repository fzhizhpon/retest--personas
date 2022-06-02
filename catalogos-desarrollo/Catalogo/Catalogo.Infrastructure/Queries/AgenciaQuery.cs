namespace Catalogo.Infrastructure.Queries
{
    public static class AgenciaQuery
    {
        public static string SelectAgenciasPorSucursal(string esquema)
        {
            return "SELECT" +
                "   a.CODIGO_AGENCIA codigo," +
                "   a.DESCRIPCION" +
                $" FROM {esquema}.SIFV_AGENCIAS a" +
                " WHERE a.CODIGO_SUCURSAL = :codigoSucursal" +
                " AND a.ESTADO = '1'";
        }
    }
}
