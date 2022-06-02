namespace Personas.Application.CodigosEventos
{
    /*CODIGOS ERROR PERSONAS: +100 / +124 || -100 / -124 */
    public static class PersonasEventos
    {
        public const int ACTUALIZAR_PERSONA_OK = 100;
        public const int ACTUALIZAR_PERSONA_NO_ACTUALIZADO = 101;
        public const int ACTUALIZAR_PERSONA_ERROR = -100;

        public const int OBTENER_PERSONA_OK = 102;
        public const int OBTENER_PERSONA_NO_DATOS = 103;
        public const int OBTENER_PERSONA_ERROR = -101;

        public const int GUARDAR_PERSONA_OK = 104;
        public const int GUARDAR_PERSONA_NO_DATOS = 105;
        public const int GUARDAR_PERSONA_ERROR = -102;

        public const int OBTENER_CODIGO_PERSONA_MAX_NO_DATOS = 106;
        public const int OBTENER_CODIGO_PERSONA_MAX_ERROR = -103;

        public const int OBTENER_PERSONAS_ERROR = -104;

        public const int ERROR_COLOCAR_FECHA_ACTUALIZACION = -105;

        public const int ERROR_CEDULA_INVALIDA = -106;
        public const int ERROR_RUC_INVALIDO = -107;

        public const int ERROR_CONTAR_PERSONAS = -108;
        public const int ERROR_PERSONA_YA_EXISTE = -109;
    }
}
