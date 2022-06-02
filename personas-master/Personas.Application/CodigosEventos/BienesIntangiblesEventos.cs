namespace Personas.Application.CodigosEventos
{
    // + 1800 - 1800
    public class BienesIntangiblesEventos
    {
        // CREATE
        public const int GUARDAR_BIENES_INTANGIBLES = 1800;
        public const int BIENES_INTANGIBLES_NO_GUARDADO = 1801;
        public const int GUARDAR_BIENES_INTANGIBLES_ERROR = -1800;

        // READ
        public const int OBTENER_BIENES_INTANGIBLES = 1802;
        public const int BIENES_INTANGIBLES_NO_OBTENIDO = 1803;
        public const int OBTENER_BIENES_INTANGIBLES_ERROR = -1801;

        // READ
        public const int OBTENER_BIEN_INTANGIBLE = 1804;
        public const int BIEN_INTANGIBLE_NO_OBTENIDO = 1805;
        public const int OBTENER_BIEN_INTANGIBLE_ERROR = -1802;

        // UPDATE
        public const int ACTUALIZAR_BIENES_INTANGIBLES = 1806;
        public const int BIENES_INTANGIBLES_NO_ACTUALIZADO = 1807;
        public const int ACTUALIZAR_BIENES_INTANGIBLES_ERROR = -1803;

        // DELETE
        public const int ELIMINAR_BIENES_INTANGIBLES = 1808;
        public const int BIENES_INTANGIBLES_NO_ELIMINADO = 1809;
        public const int ELIMINAR_BIENES_INTANGIBLES_ERROR = -1804;

        // GET ID MAX
        public const int OBTENER_NUMERO_REGISTRO_MAX = 1810;
        public const int OBTENER_NUMERO_REGISTRO_MAX_ERROR = -1805;
    }
}