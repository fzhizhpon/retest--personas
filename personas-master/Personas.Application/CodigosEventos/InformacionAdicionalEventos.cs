namespace Personas.Application.CodigosEventos
{
    // + 1900 - 1900
    public class InformacionAdicionalEventos
    {
        // CREATE
        public const int GUARDAR_INFORMACION_ADICIONAL = 1900;
        public const int INFORMACION_ADICIONAL_NO_GUARDADO = 1901;
        public const int GUARDAR_INFORMACION_ADICIONAL_ERROR = -1900;

        // READ
        public const int OBTENER_INFORMACION_ADICIONAL = 1902;
        public const int INFORMACION_ADICIONAL_NO_OBTENIDO = 1903;
        public const int OBTENER_INFORMACION_ADICIONAL_ERROR = -1901;
        
        // UPDATE
        public const int ACTUALIZAR_INFORMACION_ADICIONAL = 1904;
        public const int INFORMACION_ADICIONAL_NO_ACTUALIZADO = 1905;
        public const int ACTUALIZAR_INFORMACION_ADICIONAL_ERROR = -1902;
    }
}