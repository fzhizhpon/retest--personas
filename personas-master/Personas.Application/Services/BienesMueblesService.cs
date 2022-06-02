using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.BienesMuebles;
using Personas.Core.Entities.BienesMuebles;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{
    public class BienesMueblesService : IBienesMueblesService
    {
        private readonly IBienesMueblesRepository _bienesMueblesRepository; // interfaz del repositorio correspondiente
        private readonly ConfiguracionApp _config; // configuracion
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ILogsRepository<BienesMuebles> _logger;

        public BienesMueblesService(
            IBienesMueblesRepository bienesMueblesRepository,
            ConfiguracionApp configuracionApp,
            ILogsRepository<BienesMuebles> logger,
            IMensajesRespuestaRepository textoInfoService
        )
        {
            _bienesMueblesRepository = bienesMueblesRepository;
            _config = configuracionApp;
            _textoInfoService = textoInfoService;
            _logger = logger;
        }

        public async Task<Respuesta> ObtenerBienesMuebles(long codigoPersona)
        {
            // * variables para la respuesta de la query
            int codigo = 0;
            IEnumerable<BienesMuebles> bienesMuebles = null;

            // * codigos de eventos
            int codigoEvento = BienesMueblesEventos.OBTENER_BIENES_MUEBLES;
            // * codigo interno de funcionamiento
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {

                try
                {
                    _logger.Informativo("Obteniendo BienesMuebles");
                    (codigo, bienesMuebles) = await _bienesMueblesRepository.ObtenerBienesMuebles(codigoPersona);
                    _logger.Informativo("Obtenido BienesMuebles");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ObtenerBienesMuebles => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerBienesMuebles=> {ex}");
                        codigoEvento = BienesMueblesEventos.OBTENER_BIENES_MUEBLES_ERROR;
                    }
                }
            }

            string textoInfo = await _textoInfoService.ObtenerTextoInfo(_config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = textoInfo,
                resultado = bienesMuebles
            };
        }

        public async Task<Respuesta> ObtenerBienMueble(long codigoPersona, long numeroRegistro)
        {
            // * variables para la respuesta de la query
            int codigo = 0;
            BienesMuebles bienMueble = null;

            // * codigos de eventos
            int codigoEvento = BienesMueblesEventos.OBTENER_BIEN_MUEBLE;
            // * codigo interno de funcionamiento
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo("Obteniendo BienMueble");
                    (codigo, bienMueble) = await _bienesMueblesRepository.ObtenerBienMueble(codigoPersona, numeroRegistro);
                    _logger.Informativo("Obtenido BienMueble");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ObtenerBienMueble => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerBienMueble=> {ex}");
                        codigoEvento = BienesMueblesEventos.OBTENER_BIEN_MUEBLE_ERROR;
                    }
                }
            }

            string textoInfo = await _textoInfoService.ObtenerTextoInfo(_config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = textoInfo,
                resultado = bienMueble
            };
        }

        public async Task<Respuesta> GuardarBienesMuebles(GuardarBienesMueblesDto obj)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                // * numero de registro
                long numeroRegistro = 0;
                // * codigos de eventos
                int codigoEvento = BienesMueblesEventos.GUARDAR_BIENES_MUEBLES;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo("Obteniendo codigoNumeroRegistro...");
                    // recuperamos el ultimo id ingresado
                    numeroRegistro = await _bienesMueblesRepository.ObtenerNumeroRegistroMax();
                    numeroRegistro += 1;
                    _logger.Informativo("Obtenido codigoNumeroRegistro...");

                    _logger.Informativo("Guardando BienesMuebles...");
                    // * insercion
                    await _bienesMueblesRepository.GuardarBienesMuebles(
                        new GuardarBienesMueblesDto()
                        {
                            codigoPersona = obj.codigoPersona,
                            numeroRegistro = numeroRegistro,
                            tipoBienMueble = obj.tipoBienMueble,
                            codigoReferencia = obj.codigoReferencia,
                            descripcion = obj.descripcion,
                            avaluoComercial = obj.avaluoComercial,
                            codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                            fechaUsuarioActualiza = DateTime.Now,
                            estado = '1'
                        }
                    );
                    _logger.Informativo("Guardado BienesMuebles...");
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"GuardarBienesMuebles => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarBienesMuebles=> {ex}");
                        codigoEvento = BienesMueblesEventos.GUARDAR_BIENES_MUEBLES_ERROR;
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

        public async Task<Respuesta> ActualizarBienesMuebles(ActualizarBienesMueblesDto obj)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                // * codigos de eventos
                int codigoEvento = BienesMueblesEventos.ACTUALIZAR_BIENES_MUEBLES;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo("Actualizando BienesMuebles");
                    int resultado = await _bienesMueblesRepository.ActualizarBienesMuebles(
                        new ActualizarBienesMueblesDto()
                        {
                            codigoPersona = obj.codigoPersona,
                            numeroRegistro = obj.numeroRegistro,
                            tipoBienMueble = obj.tipoBienMueble,
                            codigoReferencia = obj.codigoReferencia,
                            descripcion = obj.descripcion,
                            avaluoComercial = obj.avaluoComercial,
                            codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                            fechaUsuarioActualiza = DateTime.Now
                        });
                    _logger.Informativo("Actualizo BienesMuebles");
                    if (resultado == 0)
                    {
                        throw new ExcepcionOperativa(BienesMueblesEventos.BIENES_MUEBLES_NO_ACTUALIZADO);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ActualizarBienesMuebles => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarBienesMuebles=> {ex}");
                        codigoEvento = BienesMueblesEventos.ACTUALIZAR_BIENES_MUEBLES_ERROR;
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

        public async Task<Respuesta> EliminarBienesMuebles(EliminarBienesMueblesDto obj)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                // * codigos de eventos
                int codigoEvento = BienesMueblesEventos.ELIMINAR_BIENES_MUEBLES;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo("Eliminando BienesMuebles");
                    int resultado = await _bienesMueblesRepository.EliminarBienesMuebles(
                        new EliminarBienesMueblesDto()
                        {
                            codigoPersona = obj.codigoPersona,
                            numeroRegistro = obj.numeroRegistro
                        });
                    _logger.Informativo("Eliminado BienesMuebles");
                    if (resultado == 0)
                    {
                        throw new ExcepcionOperativa(BienesMueblesEventos.BIENES_MUEBLES_NO_ELIMINADO);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"EliminarBienesMuebles => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"EliminarBienesMuebles=> {ex}");
                        codigoEvento = BienesMueblesEventos.ELIMINAR_BIENES_MUEBLES_ERROR;
                    }
                }

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                    _config.Idioma, codigoEvento, _config.Modulo);

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