namespace Personas.Application.CodigosEventos
{
    // + 1700 - 1700
    public class BienesInmueblesEventos
    {
        // CREATE
        public const int GUARDAR_BIENES_INMUEBLES = 1700;
        public const int BIENES_INMUEBLES_NO_GUARDADO = 1701;
        public const int GUARDAR_BIENES_INMUEBLES_ERROR = -1700;

        // READ
        public const int OBTENER_BIENES_INMUEBLES = 1702;
        public const int BIENES_INMUEBLES_NO_OBTENIDO = 1703;
        public const int OBTENER_BIENES_INMUEBLES_ERROR = -1701;

        // READ
        public const int OBTENER_BIEN_INMUEBLE = 1704;
        public const int BIEN_INMUEBLE_NO_OBTENIDO = 1705;
        public const int OBTENER_BIEN_INMUEBLE_ERROR = -1702;

        // UPDATE
        public const int ACTUALIZAR_BIENES_INMUEBLES = 1706;
        public const int BIENES_INMUEBLES_NO_ACTUALIZADO = 1707;
        public const int ACTUALIZAR_BIENES_INMUEBLES_ERROR = -1703;

        // DELETE
        public const int ELIMINAR_BIENES_INMUEBLES = 1708;
        public const int BIENES_INMUEBLES_NO_ELIMINADO = 1709;
        public const int ELIMINAR_BIENES_INMUEBLES_ERROR = -1704;

        // GET ID MAX
        public const int OBTENER_NUMERO_REGISTRO_MAX = 1710;
        public const int OBTENER_NUMERO_REGISTRO_MAX_ERROR = -1705;
    }
}