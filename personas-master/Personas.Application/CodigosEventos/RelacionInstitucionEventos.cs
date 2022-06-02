namespace Personas.Application.CodigosEventos
{
    // + 2000 - 2000
    public class RelacionInstitucionEventos
    {
        // CREATE
        public const int GUARDAR_RELACION_INSTITUCION = 2000;
        public const int RELACION_INSTITUCION_NO_GUARDADO = 2001;
        public const int GUARDAR_RELACION_INSTITUCION_ERROR = -2000;

        // READ
        public const int OBTENER_RELACION_INSTITUCION = 2002;
        public const int RELACION_INSTITUCION_NO_OBTENIDO = 2003;
        public const int OBTENER_RELACION_INSTITUCION_ERROR = -2001;


        // UPDATE
        public const int ACTUALIZAR_RELACION_INSTITUCION = 2006;
        public const int RELACION_INSTITUCION_NO_ACTUALIZADO = 2007;
        public const int ACTUALIZAR_RELACION_INSTITUCION_ERROR = -2003;

        // DELETE
        public const int ELIMINAR_RELACION_INSTITUCION = 2008;
        public const int RELACION_INSTITUCION_NO_ELIMINADO = 2009;
        public const int ELIMINAR_RELACION_INSTITUCION_ERROR = -2004;


    }
}