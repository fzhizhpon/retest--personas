namespace Personas.Application.CodigosEventos
{
    // + 1300 - 1300
    public static class FamiliaresEventos
    {
        public const int GUARDAR_FAMILIAR = 1300;
        public const int FAMILIAR_NO_GUARDADO = 1301;
        public const int GUARDAR_FAMILIAR_ERROR = -1300;

        public const int OBTENER_FAMILIAR = 1302;
        public const int FAMILIAR_NO_OBTENIDO = 1303;
        public const int OBTENER_FAMILIAR_ERROR = -1301;

        public const int OBTENER_VARIOS_FAMILIARES = 1304;
        public const int VARIOS_FAMILIARES_NO_OBTENIDOS = 1305;
        public const int OBTENER_VARIOS_FAMILIARES_ERROR = -1302;

        public const int ELIMINAR_FAMILIAR = 1306;
        public const int FAMILIAR_NO_ELIMINADO = 1307;
        public const int ELIMINAR_FAMILIAR_ERROR = -1303;

        public const int ACTUALIZAR_FAMILIAR = 1308;
        public const int FAMILIAR_NO_ACTUALIZADO = 1309;
        public const int ACTUALIZAR_FAMILIAR_ERROR = -1304;

        public const int ACTUALIZAR_FAMILIAR_PRINCIPAL_ERROR = -1305;
        public const int ACTUALIZAR_FAMILIAR_PRINCIPAL_EXISTENCIA_ERROR = -1306;
        public const int ACTUALIZAR_FAMILIAR_ELIMINAR_PRINCIPAL_ERROR = -1307;

        public const int COD_PERSONA_IGUAL_FAMILIAR = -1308;
    }
}
