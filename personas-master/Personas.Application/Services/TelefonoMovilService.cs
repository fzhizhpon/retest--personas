using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.TelefonoMovil;
using Personas.Core.Entities.TelefonosMovil;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Personas.Application.Services
{
    public class TelefonoMovilService : ITelefonoMovilService
    {
        protected readonly ITelefonoMovilRepository _telefonoMovilRepository;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<ReferenciasComercialesService> _logger;

        public TelefonoMovilService(ITelefonoMovilRepository repository,
            IMensajesRespuestaRepository textoInfoService,
            ILogsRepository<ReferenciasComercialesService> logger,
            ConfiguracionApp config)
        {
            _telefonoMovilRepository = repository;
            _textoInfoService = textoInfoService;
            _config = config;
            _logger = logger;
        }

        public async Task<Respuesta> GuardarTelefonoMovil(GuardarTelefonoMovilDto dto)
        {
            (int codigo, int resultado) = await _telefonoMovilRepository.GuardarTelefonoMovil(dto);

            int codigoEvento = TelefonosMovilesEventos.GUARDAR_TELEFONO_MOVIL; // Se guardó telefono movil

            if (codigo == CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO)
            {
                if (resultado == 0)
                {
                    codigoEvento = TelefonosMovilesEventos.TELEFONO_MOVIL_NO_GUARDADO; // No se guardó telefono movil
                }
            }
            else
            {
                codigoEvento = TelefonosMovilesEventos.GUARDAR_TELEFONO_MOVIL_ERROR; // Ocurrio un error al guardar telefono movil
            }

            // ValoresConstantes.informacionToken.idioma
            // llamar a mongo para obtener el mensaje segun el idioma y tipo de codigo

            _logger.Informativo($"GuardarTelefonoMovil => {codigoEvento}");


            string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigo,
                mensaje = textoInfo,
                resultado = null
            };
        }

        public async Task<Respuesta> ObtenerTelefonosMovil(ObtenerTelefonosMovilDto dto)
        {
            int codigoEvento = TelefonosMovilesEventos.OBTENER_VARIOS_TELEFONO_MOVILES; // Se obtuvo telefonos moviles

            (int codigo, IEnumerable<TelefonoMovil> telefonos) =
                await _telefonoMovilRepository.ObtenerTelefonosMovil(dto);

            if (codigo == CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO)
            {
                if (telefonos == null)
                {
                    codigoEvento = TelefonosMovilesEventos.VARIOS_TELEFONOS_MOVILES_NO_OBTENIDOS; // No se encontró telefonos moviles
                }
            }
            else
            {
                codigoEvento = TelefonosMovilesEventos.OBTENER_VARIOS_TELEFONO_MOVILES_ERROR; // Ocurrio un error al obtener telefonos moviles
            }

            // ValoresConstantes.informacionToken.idioma
            // llamar a mongo para obtener el mensaje segun el idioma y tipo de codigo

            _logger.Informativo($"ObtenerTelefonosMovil => {codigoEvento}");

            string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigo,
                mensaje = textoInfo,
                resultado = telefonos
            };
        }

        public async Task<Respuesta> ActualizarTelefonoMovil(ActualizarTelefonoMovilDto dto)
        {
            (int codigo, int resultado) = await _telefonoMovilRepository.ActualizarTelefonoMovil(dto);

            int codigoEvento = TelefonosMovilesEventos.ACTUALIZAR_TELEFONO_MOVIL; // Se actilizó telefono movil

            if (codigo == CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO)
            {
                if (resultado == 0)
                {
                    codigoEvento = TelefonosMovilesEventos.TELEFONO_MOVIL_NO_ACTUALIZADO; // No se actilizó telefono movil
                }
            }
            else
            {
                codigoEvento = TelefonosMovilesEventos.ACTUALIZAR_TELEFONO_MOVIL_ERROR; // Ocurrio un error al actilizar telefono movil
            }

            // ValoresConstantes.informacionToken.idioma
            // llamar a mongo para obtener el mensaje segun el idioma y tipo de codigo

            _logger.Informativo($"ActualizarTelefonoMovil => {codigoEvento}");


            string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigo,
                mensaje = textoInfo,
                resultado = null
            };
        }

        public async Task<Respuesta> EliminarTelefonoMovil(EliminarTelefonoMovilDto dto)
        {
            (int codigo, int resultado) = await _telefonoMovilRepository.EliminarTelefonoMovil(dto);

            int codigoEvento = TelefonosMovilesEventos.ELIMINAR_TELEFONO_MOVIL; // Se eliminó telefono movil

            if (codigo == CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO)
            {
                if (resultado == 0)
                {
                    codigoEvento = TelefonosMovilesEventos.TELEFONO_MOVIL_NO_ELIMINADO; // No se eliminó telefono movil
                }
            }
            else
            {
                codigoEvento = TelefonosMovilesEventos.ELIMINAR_TELEFONO_MOVIL_ERROR; // Ocurrio un error al eliminar telefono movil
            }

            // ValoresConstantes.informacionToken.idioma
            // llamar a mongo para obtener el mensaje segun el idioma y tipo de codigo

            _logger.Informativo($"EliminarTelefonoMovil => {codigoEvento}");


            string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigo,
                mensaje = textoInfo,
                resultado = null
            };
        }
    }
}
