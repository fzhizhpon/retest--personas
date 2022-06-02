namespace Personas.Application.EnumsEventos
{
    // + 500 - 500
    public static class DireccionesEventos
    {
        public const int GUARDADO_OK = 500;
        public const int ACTUALIZADO_OK = 501;
        public const int ELIMINADO_OK = 502;
        public const int LEIDO_OK = 503;
        public const int LEIDOS_VARIOS_OK = 504;

        public const int ERROR_GENERAR_CODIGO = -500;
        public const int ERROR_INSERTAR_DIRECCION = -501;
        public const int ERROR_DESCMARCAR_PRINCIPAL = -502;
        public const int ERROR_GUARDAR_DIRECCION = -503;
        public const int ERROR_CONTAR_PRINCIPALES = -504;

        public const int ERROR_ELIMINAR_DIRECCION = -505;

        public const int ERROR_OBTENER_DIRECCIONES = -506;
        public const int ERROR_OBTENER_DIRECCION = -507;

        public const int ERROR_ACTUALIZAR_DIRECCION = -508;
    }
}
