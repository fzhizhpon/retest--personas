namespace Personas.Application.CodigosEventos
{

    // + 1400 - 1400
    public static class EstadosFinancierosEventos
    {
        public const int GUARDAR_ESTADO_FINANCIERO = 1400;
        public const int ESTADO_FINANCIERO_NO_GUARDADO = 1401;
        public const int GUARDAR_ESTADO_FINANCIERO_ERROR = -1400;

        public const int ACTUALIZAR_ESTADO_FINANCIERO = 1408;
        public const int ESTADO_FINANCIERO_NO_ACTUALIZADO = 1409;
        public const int ACTUALIZAR_ESTADO_FINANCIERO_ERROR = -1404;

        public const int OBTENER_CUENTAS_ESTADO_FINANCIERO = 1402;
        public const int CUENTAS_ESTADO_FINANCIERO_NO_OBTENIDAS = 1403;
        public const int OBTENER_CUENTAS_ESTADO_FINANCIERO_ERROR = -1401;

        public const int OBTENER_VALOR_CUENTA_POR_QUERY_ERROR = -1402;
    }
}
