using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personas.Application.CodigosEventos
{
    public class TelefonosMovilesEventos
    {

        public const int GUARDAR_TELEFONO_MOVIL = 200;
        public const int TELEFONO_MOVIL_NO_GUARDADO = 201;
        public const int GUARDAR_TELEFONO_MOVIL_ERROR = -200;

        public const int OBTENER_VARIOS_TELEFONO_MOVILES = 202;
        public const int VARIOS_TELEFONOS_MOVILES_NO_OBTENIDOS = 203;
        public const int OBTENER_VARIOS_TELEFONO_MOVILES_ERROR = -201;

        public const int ACTUALIZAR_TELEFONO_MOVIL = 204;
        public const int TELEFONO_MOVIL_NO_ACTUALIZADO = 205;
        public const int ACTUALIZAR_TELEFONO_MOVIL_ERROR = -202;

        public const int ELIMINAR_TELEFONO_MOVIL = 206;
        public const int TELEFONO_MOVIL_NO_ELIMINADO = 207;
        public const int ELIMINAR_TELEFONO_MOVIL_ERROR = -203;

    }
}
