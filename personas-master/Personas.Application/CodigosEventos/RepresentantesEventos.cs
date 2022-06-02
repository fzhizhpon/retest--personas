namespace Personas.Application.CodigosEventos
{

    // + 1200 - 1200
    public static class RepresentantesEventos
    {
        public const int GUARDAR_REPRESENTANTE = 1200;
        public const int REPRESENTANTE_NO_GUARDADO = 1201;
        public const int GUARDAR_REPRESENTANTE_ERROR = -1200;

        public const int OBTENER_REPRESENTANTE = 1202;
        public const int REPRESENTANTE_NO_OBTENIDO = 1203;
        public const int OBTENER_REPRESENTANTE_ERROR = -1201;

        public const int OBTENER_VARIOS_REPRESENTANTES = 1204;
        public const int VARIOS_REPRESENTANTES_NO_OBTENIDOS = 1205;
        public const int OBTENER_VARIOS_REPRESENTANTES_ERROR = -1202;

        public const int ELIMINAR_REPRESENTANTE = 1206;
        public const int REPRESENTANTE_NO_ELIMINADO = 1207;
        public const int ELIMINAR_REPRESENTANTE_ERROR = -1203;

        public const int ACTUALIZAR_REPRESENTANTE = 1208;
        public const int REPRESENTANTE_NO_ACTUALIZADO = 1209;
        public const int ACTUALIZAR_REPRESENTANTE_ERROR = -1204;

        public const int ACTUALIZAR_REPRESENTANTE_PRINCIPAL_ERROR = -1205;
        public const int ACTUALIZAR_REPRESENTANTE_PRINCIPAL_EXISTENCIA_ERROR = -1206;
        public const int ACTUALIZAR_REPRESENTANTE_ELIMINAR_PRINCIPAL_ERROR = -1207;
        public const int GUARDAR_REPRESENTANTE_ERROR_AUTO_REPRESENTACION = -1208;

    }
}
