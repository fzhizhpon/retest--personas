namespace Personas.Application.CodigosEventos
{
    /*CODIGOS ERROR PERSONAS NATURALES: +125 / +149 || -125 / -149 */
    public static class PersonasNaturalesEventos
    {
        public const int GUARDAR_PERSONA_NATURAL_OK = 125;
        public const int GUARDAR_PERSONA_NATURAL_NO_GUARDADO = 126;
        public const int GUARDAR_PERSONA_NATURAL_ERROR = -125;


        public const int OBTENER_INFO_PERSONA_NATURAL_OK = 127;
        public const int OBTENER_INFO_PERSONA_NATURAL_NO_DATOS = 128;
        public const int OBTENER_INFO_PERSONA_NATURAL_ERROR = -126;

        public const int OBTENER_PERSONA_NATURAL_OK = 129;
        public const int OBTENER_PERSONA_NATURAL_NO_DATOS = 130;
        public const int OBTENER_PERSONA_NATURAL_ERROR = -127;

        public const int ACTUALIZAR_PERSONA_NATURAL_OK = 131;
        public const int ACTUALIZAR_PERSONA_NATURAL_NO_ACTUALIZADO = 132;
        public const int ACTUALIZAR_PERSONA_NATURAL_ERROR = -128;

        public const int ACTUALIZAR_CONYUGUE_ERROR = -129;
    }
}
