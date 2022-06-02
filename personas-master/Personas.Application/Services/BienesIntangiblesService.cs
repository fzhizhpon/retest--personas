using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Core.App;
using Personas.Application.CodigosEventos;
using Personas.Core.Dtos.BienesIntangibles;
using Personas.Core.Entities.BienesIntangibles;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{
    public class BienesIntangiblesService : IBienesIntangiblesService

    {
        private readonly IBienesIntangiblesRepository
            _bienesIntangiblesRepository; // interfaz del repositorio correspondiente

        private readonly ConfiguracionApp _config; // configuracion

        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ILogsRepository<BienesIntangibles> _logger;

        public BienesIntangiblesService(
            IBienesIntangiblesRepository bienesIntangiblesRepository,
            ConfiguracionApp config,
            ILogsRepository<BienesIntangibles> logger,
            IMensajesRespuestaRepository textoInfoService
        )
        {
            _bienesIntangiblesRepository = bienesIntangiblesRepository;
            _config = config;
            _textoInfoService = textoInfoService;
            _logger = logger;
        }


        public async Task<Respuesta> ObtenerBienesIntangibles(long codigoPersona)
        {
            // * variables para la respuesta de la query
            int codigo = 0;
            IEnumerable<BienesIntangibles> bienesIntangibles = null;

            // * codigos de eventos
            int codigoEvento = BienesIntangiblesEventos.OBTENER_BIENES_INTANGIBLES;
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
                    _logger.Informativo("Obteniendo BienesIntangibles");
                    (codigo, bienesIntangibles) =
                        await _bienesIntangiblesRepository.ObtenerBienesIntangibles(codigoPersona);
                    _logger.Informativo("Obtenido BienesIntangibles");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ObtenerBienesIntangibles => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerBienesIntangibles=> {ex}");
                        codigoEvento = BienesIntangiblesEventos.OBTENER_BIENES_INTANGIBLES_ERROR;
                    }
                }
            }

            string textoInfo = await _textoInfoService.ObtenerTextoInfo(_config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = textoInfo,
                resultado = bienesIntangibles
            };
        }

        public async Task<Respuesta> ObtenerBienIntangible(long codigoPersona, long numeroRegistro)
        {
            // * variables para la respuesta de la query
            int codigo = 0;
            BienesIntangibles bienIntangible = null;

            // * codigos de eventos
            int codigoEvento = BienesIntangiblesEventos.OBTENER_BIEN_INTANGIBLE;
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
                    _logger.Informativo("Obteniendo BienIntangible");
                    (codigo, bienIntangible) =
                        await _bienesIntangiblesRepository.ObtenerBienIntangible(codigoPersona, numeroRegistro);
                    _logger.Informativo("Obtenido BienIntangible");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ObtenerBienIntangible => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerBienIntangible=> {ex}");
                        codigoEvento = BienesIntangiblesEventos.OBTENER_BIEN_INTANGIBLE_ERROR;
                    }
                }
            }

            string textoInfo = await _textoInfoService.ObtenerTextoInfo(_config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = textoInfo,
                resultado = bienIntangible
            };
        }

        public async Task<Respuesta> GuardarBienesIntangibles(GuardarBienesIntangiblesDto obj)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                // * numero de registro
                long numeroRegistro = 0;
                // * codigos de eventos
                int codigoEvento = BienesIntangiblesEventos.GUARDAR_BIENES_INTANGIBLES;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo("Obteniendo codigoNumeroRegistro...");
                    // recuperamos el ultimo id ingresado
                    numeroRegistro = await _bienesIntangiblesRepository.ObtenerNumeroRegistroMax();
                    numeroRegistro += 1;
                    _logger.Informativo("Obtenido codigoNumeroRegistro...");

                    _logger.Informativo("Guardando BienesIntangibles...");
                    // * insercion
                    await _bienesIntangiblesRepository.GuardarBienesIntangibles(
                        new GuardarBienesIntangiblesDto()
                        {
                            codigoPersona = obj.codigoPersona,
                            numeroRegistro = numeroRegistro,
                            tipoBienIntangible = obj.tipoBienIntangible,
                            codigoReferencia = obj.codigoReferencia,
                            descripcion = obj.descripcion,
                            avaluoComercial = obj.avaluoComercial,
                            codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                            fechaUsuarioActualiza = DateTime.Now,
                            estado = '1'
                        }
                    );

                    _logger.Informativo("Guardado BienesIntangibles...");
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"GuardarBienesIntangibles => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarBienesIntangibles=> {ex}");
                        codigoEvento = BienesIntangiblesEventos.GUARDAR_BIENES_INTANGIBLES_ERROR;
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

        public async Task<Respuesta> ActualizarBienesIntangibles(ActualizarBienesIntangiblesDto obj)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                // * codigos de eventos
                int codigoEvento = BienesIntangiblesEventos.ACTUALIZAR_BIENES_INTANGIBLES;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo("Actualizando BienesIntangibles");
                    int resultado = await _bienesIntangiblesRepository.ActualizarBienesIntangibles(
                        new ActualizarBienesIntangiblesDto()
                        {
                            codigoPersona = obj.codigoPersona,
                            numeroRegistro = obj.numeroRegistro,
                            descripcion = obj.descripcion,
                            avaluoComercial = obj.avaluoComercial,
                            codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                            fechaUsuarioActualiza = DateTime.Now
                        });
                    _logger.Informativo("Actualizado BienesIntangibles");
                    if (resultado == 0)
                    {
                        throw new ExcepcionOperativa(BienesIntangiblesEventos.BIENES_INTANGIBLES_NO_ACTUALIZADO);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ActualizarBienesIntangibles => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarBienesIntangibles=> {ex}");
                        codigoEvento = BienesIntangiblesEventos.ACTUALIZAR_BIENES_INTANGIBLES_ERROR;
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

        public async Task<Respuesta> EliminarBienesIntangibles(EliminarBienesIntangiblesDto obj)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                // * codigos de eventos
                int codigoEvento = BienesIntangiblesEventos.ELIMINAR_BIENES_INTANGIBLES;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
                try
                {
                    _logger.Informativo("Eliminando BienesIntangibles");
                    int resultado = await _bienesIntangiblesRepository.EliminarBienesIntangibles(
                        new EliminarBienesIntangiblesDto()
                        {
                            codigoPersona = obj.codigoPersona,
                            numeroRegistro = obj.numeroRegistro
                        });
                    _logger.Informativo("Eliminado BienesIntangibles");
                    if (resultado == 0)
                    {
                        throw new ExcepcionOperativa(BienesIntangiblesEventos.BIENES_INTANGIBLES_NO_ELIMINADO);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"EliminarBienesIntangibles => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"EliminarBienesIntangibles=> {ex}");
                        codigoEvento = BienesIntangiblesEventos.ELIMINAR_BIENES_INTANGIBLES_ERROR;
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