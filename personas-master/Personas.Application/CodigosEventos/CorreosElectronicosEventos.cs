namespace Personas.Application.CodigosEventos
{
    public static class CorreosElectronicosEventos
	{
        public const int GUARDADO_OK = 600;
        public const int ELIMINADO_OK = 601;
        public const int ACTUALIZADO_OK = 602;
        public const int LEIDO_OK = 603;

        public const int ERROR_GENERAR_CODIGO = -600;
        public const int ERROR_DESMARCAR_PRINCIPAL = -601;
        public const int ERROR_CONTAR_PRINCIPALES = -603;
        public const int ERROR_INSERTAR_CORREO = -604;
        public const int ERROR_ELIMINAR_CORREO = -605;

        public const int ERROR_OBTENER_CORREOS = -606;
        public const int ERROR_GUARDAR_CORREO = -607;

        public const int ERROR_OBTENER_CORREO = -608;
        public const int ERROR_ACTUALIZAR_CORREO = -609;
        public const int ERROR_CORREO_EXISTE = -610;
    }
}

