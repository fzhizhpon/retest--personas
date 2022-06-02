namespace Personas.Application.CodigosEventos
{
    // +400 -400

    public static class TrabajosEventos
    {

        public const int GUARDAR_TRABAJO = 400;
        public const int TRABAJO_NO_GUARDADO = 401;
        public const int GUARDAR_TRABAJO_ERROR = -400;

        public const int OBTENER_TRABAJO = 402;
        public const int TRABAJO_NO_OBTENIDO = 403;
        public const int OBTENER_TRABAJO_ERROR = -401;

        public const int OBTENER_VARIOS_TRABAJOS = 404;
        public const int VARIOS_TRABAJOS_NO_OBTENIDOS = 405;
        public const int OBTENER_VARIOS_TRABAJOS_ERROR = -402;

        public const int ELIMINAR_TRABAJO = 406;
        public const int TRABAJO_NO_ELIMINADO = 407;
        public const int ELIMINAR_TRABAJO_ERROR = -403;

        public const int ACTUALIZAR_TRABAJO = 408;
        public const int TRABAJO_NO_ACTUALIZADO = 409;
        public const int ACTUALIZAR_TRABAJO_ERROR = -404;

    }
}
