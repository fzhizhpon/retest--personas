using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.TablasComunes;
using Personas.Core.Entities.TablasComunes;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{
    public class InformacionAdicionalService : IInformacionAdicionalService
    {
        private readonly IInformacionAdicionalRepository
            _tablasComunesDetallesRepository; // interfaz del repositorio correspondiente

        private readonly ConfiguracionApp _config; // configuracion
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ILogsRepository<InformacionAdicional> _logger;

        public InformacionAdicionalService(
            IInformacionAdicionalRepository tablasComunesDetallesRepository,
            ConfiguracionApp config,
            IMensajesRespuestaRepository textoInfoService,
            ILogsRepository<InformacionAdicional> logger)
        {
            _tablasComunesDetallesRepository = tablasComunesDetallesRepository;
            _config = config;
            _textoInfoService = textoInfoService;
            _logger = logger;
        }

        public async Task<Respuesta> ObtenerInformacionAdicional(long codigoPersona, long codigoTabla)
        {
            // * variables para la respuesta de la query
            int codigo = 0;
            IEnumerable<InformacionAdicional> tablasComunesDetalles = null;

            // * codigos de eventos
            int codigoEvento = InformacionAdicionalEventos.OBTENER_INFORMACION_ADICIONAL;
            // * codigo interno de funcionamiento
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                       TransactionScopeOption.Required,
                       new TransactionOptions {IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted},
                       TransactionScopeAsyncFlowOption.Enabled
                   ))
            {
                try
                {
                    _logger.Informativo("Obteniendo InformacionAdicional");
                    (codigo, tablasComunesDetalles) =
                        await _tablasComunesDetallesRepository.ObtenerInformacionAdicional(codigoPersona, codigoTabla);
                    _logger.Informativo("Obtenido InformacionAdicional");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa) ex;
                        _logger.Error($"ObtenerInformacionAdicional => {exo}");
                        codigoEvento = ((ExcepcionOperativa) ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerInformacionAdicional=> {ex}");
                        codigoEvento = InformacionAdicionalEventos.OBTENER_INFORMACION_ADICIONAL_ERROR;
                    }
                }

                string textoInfo =
                    await _textoInfoService.ObtenerTextoInfo(_config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta()
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = tablasComunesDetalles
                };
            }
        }

        public async Task<Respuesta> GuardarInformacionAdicional(GuardarInformacionAdicionalDto obj)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
                   {
                       IsolationLevel = IsolationLevel.ReadCommitted
                   }, TransactionScopeAsyncFlowOption.Enabled))
            {
                // * codigos de eventos
                int codigoEvento = InformacionAdicionalEventos.GUARDAR_INFORMACION_ADICIONAL;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo("Guardando InformacionAdicional...");
                    await _tablasComunesDetallesRepository.GuardarInformacionAdicional(
                        new GuardarInformacionAdicionalDto()
                        {
                            codigoPersona = obj.codigoPersona,
                            codigoTabla = obj.codigoTabla,
                            codigoElemento = obj.codigoElemento,
                            observacion = obj.observacion,
                            estado = '1'
                        });

                    _logger.Informativo("Guardado InformacionAdicional...");
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa) ex;
                        _logger.Error($"GuardarInformacionAdicional => {exo}");
                        codigoEvento = ((ExcepcionOperativa) ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarInformacionAdicional=> {ex}");
                        codigoEvento = InformacionAdicionalEventos.GUARDAR_INFORMACION_ADICIONAL_ERROR;
                    }
                }

                string textoInfo =
                    await _textoInfoService.ObtenerTextoInfo(_config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta()
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = null
                };
            }
        }

        public async Task<Respuesta> ActualizarInformacionAdicional(ActualizarInformacionAdicionalDto obj)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
                   {
                       IsolationLevel = IsolationLevel.ReadCommitted
                   }, TransactionScopeAsyncFlowOption.Enabled))
            {
                // * codigos de eventos
                int codigoEvento = InformacionAdicionalEventos.ACTUALIZAR_INFORMACION_ADICIONAL;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo("Actualizando InformacionAdicional");
                    int resultado = await _tablasComunesDetallesRepository.ActualizarInformacionAdicional(
                        new ActualizarInformacionAdicionalDto()
                        {
                            codigoPersona = obj.codigoPersona,
                            codigoTabla = obj.codigoTabla,
                            codigoElemento = obj.codigoElemento,
                            observacion = obj.observacion,
                            estado = obj.estado
                        });
                    _logger.Informativo("Actualizo InformacionAdicional");
                    if (resultado == 0)
                    {
                        throw new ExcepcionOperativa(InformacionAdicionalEventos.INFORMACION_ADICIONAL_NO_ACTUALIZADO);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa) ex;
                        _logger.Error($"ActualizarInformacionAdicional => {exo}");
                        codigoEvento = ((ExcepcionOperativa) ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarInformacionAdicional=> {ex}");
                        codigoEvento = InformacionAdicionalEventos.ACTUALIZAR_INFORMACION_ADICIONAL_ERROR;
                    }
                }

                string textoInfo =
                    await _textoInfoService.ObtenerTextoInfo(_config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta()
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = null
                };
            }
        }
    }
}