namespace Personas.Application.CodigosEventos
{
    // + 1600 - 1600
    public class BienesMueblesEventos
    {
        // CREATE
        public const int GUARDAR_BIENES_MUEBLES = 1600;
        public const int BIENES_MUEBLES_NO_GUARDADO = 1601;
        public const int GUARDAR_BIENES_MUEBLES_ERROR = -1600;

        // READ
        public const int OBTENER_BIENES_MUEBLES = 1602;
        public const int BIENES_MUEBLES_NO_OBTENIDO = 1603;
        public const int OBTENER_BIENES_MUEBLES_ERROR = -1601;

        // READ
        public const int OBTENER_BIEN_MUEBLE = 1604;
        public const int BIEN_MUEBLE_NO_OBTENIDO = 1605;
        public const int OBTENER_BIEN_MUEBLE_ERROR = -1602;

        // UPDATE
        public const int ACTUALIZAR_BIENES_MUEBLES = 1606;
        public const int BIENES_MUEBLES_NO_ACTUALIZADO = 1607;
        public const int ACTUALIZAR_BIENES_MUEBLES_ERROR = -1603;

        // DELETE
        public const int ELIMINAR_BIENES_MUEBLES = 1608;
        public const int BIENES_MUEBLES_NO_ELIMINADO = 1609;
        public const int ELIMINAR_BIENES_MUEBLES_ERROR = -1604;

        // GET ID MAX
        public const int OBTENER_NUMERO_REGISTRO_MAX = 1610;
        public const int OBTENER_NUMERO_REGISTRO_MAX_ERROR = -1605;
    }
}