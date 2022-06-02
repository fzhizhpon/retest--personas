namespace Personas.Application.CodigosEventos
{
    /*CODIGOS ERROR PERSONAS NATURALES: +150 / +174 || -150 / -174 */
    public static class PersonasNoNaturalesEventos
    {
        public const int GUARDAR_PERSONA_NO_NATURAL_OK = 150;
        public const int GUARDAR_PERSONA_NO_NATURAL_NO_GUARDADO = 151;
        public const int GUARDAR_PERSONA_NO_NATURAL_ERROR = -150;

        public const int OBTENER_PERSONA_NO_NATURAL_OK = 152;
        public const int OBTENER_PERSONA_NO_NATURAL_NO_DATOS = 153;
        public const int OBTENER_PERSONA_NO_NATURAL_ERROR = -151;

        public const int ACTUALIZAR_PERSONA_NO_NATURAL_OK = 154;
        public const int ACTUALIZAR_PERSONA_NO_NATURAL_NO_ACTUALIZADO = 155;
        public const int ACTUALIZAR_PERSONA_NO_NATURAL_ERROR = -152;
    }
}
