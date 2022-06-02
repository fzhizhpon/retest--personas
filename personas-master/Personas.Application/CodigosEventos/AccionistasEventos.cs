namespace Personas.Application.CodigosEventos
{
    /*CODIGOS ERROR ACCIONISTAS: +1100 || -1100 */
    public static class AccionistasEventos
    {
        public const int GUARDAR_LISTA_ACCIONISTAS_OK = 1100;
        public const int GUARDAR_LISTA_ACCIONISTAS_NO_GUARDADO = 1101;
        public const int GUARDAR_LISTA_ACCIONISTAS_ERROR = -1100;

        public const int ACTUALIZAR_ACCIONISTA_OK = 1102;
        public const int ACTUALIZAR_ACCIONISTA_NO_GUARDADO = 1103;
        public const int ACTUALIZAR_ACCIONISTA_ERROR = -1101;

        public const int OBTENER_ACCIONISTA_OK = 1104;
        public const int OBTENER_ACCIONISTA_NO_ENCONTRADO = 1105;
        public const int OBTENER_ACCIONISTA_ERROR = -1102;
    }
}
