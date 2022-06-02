using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.Personas;
using Personas.Core.Dtos.ReferenciasPersonales;
using Personas.Core.Entities.ReferenciasPersonales;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{

    // +700 -700

    public class ReferenciasPersonalesService : IReferenciasPersonalesService
    {
        protected readonly ConfiguracionApp _config;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ILogsRepository<ReferenciasPersonalesService> _logger;
        protected readonly IReferenciasPersonalesRepository _referenciasPersonalesRepository;
        private readonly IPersonaRepository _personaRepository;

        public ReferenciasPersonalesService(
            ConfiguracionApp config,
            IMensajesRespuestaRepository textoInfoService,
            ILogsRepository<ReferenciasPersonalesService> logger,
            IReferenciasPersonalesRepository serviceReferenciasPersonalesRepository,
            IPersonaRepository personaRepository
            )
        {
            _config = config;
            _logger = logger;
            _textoInfoService = textoInfoService;
            _referenciasPersonalesRepository = serviceReferenciasPersonalesRepository;
            _personaRepository = personaRepository;
        }

        public async Task<Respuesta> GuardarReferenciaPersonal(GuardarReferenciaPersonalDto dto)
        {
            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;

            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            int codigoEvento = ReferenciasPersonalesEventos.GUARDAR_REFERENCIA_PERSONAL;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Guardando referencia personal...");

                    dto.numeroRegistro = await _referenciasPersonalesRepository.ObtenerCodigoReferenciaFinanciera(dto.codigoPersona);

                    int result = await _referenciasPersonalesRepository.GuardarReferenciaPersonal(dto);

                    if (result == 0)
                    {
                        codigoEvento = ReferenciasPersonalesEventos.REFERENCIA_PERSONAL_NO_GUARDADA;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    await _personaRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest
                    {
                        codigoPersona = dto.codigoPersona,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra
                    });

                    scope.Complete();

                    _logger.Informativo($"Referencia personal guardada");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"GuardarReferenciaPersonal => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarReferenciaPersonal => {exc}");
                        codigoEvento = ReferenciasPersonalesEventos.REFERENCIA_PERSONAL_NO_GUARDADA;
                    }
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

        public async Task<Respuesta> ObtenerReferenciaPersonal(ObtenerReferenciaPersonalDto dto)
        {
            ReferenciaPersonal.ReferenciaPersonalJoin refPersonal = null;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            int codigoEvento = ReferenciasPersonalesEventos.OBTENER_REFERENCIA_PERSONAL;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando referencia personal...");

                    refPersonal = await _referenciasPersonalesRepository.ObtenerReferenciaPersonalJoin(dto);
                    scope.Complete();

                    if (refPersonal == null)
                    {
                        codigoEvento = ReferenciasPersonalesEventos.REFERENCIA_PERSONAL_NO_OBTENIDA;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    _logger.Informativo($"Referencia personal consultada");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerReferenciaPersonalJoin => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerReferenciaPersonalJoin => {exc}");
                        codigoEvento = ReferenciasPersonalesEventos.OBTENER_REFERENCIA_PERSONAL_ERROR;
                    }
                }
            }

            string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = textoInfo,
                resultado = refPersonal
            };
        }

        public async Task<Respuesta> ObtenerReferenciasPersonales(ObtenerReferenciasPersonalesDto dto)
        {

            int codigoEvento = ReferenciasPersonalesEventos.OBTENER_VARIAS_REFERENCIAS_PERSONALES;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            IList<ReferenciaPersonal.ReferenciaPersonalMinimo> refPersonales = new List<ReferenciaPersonal.ReferenciaPersonalMinimo>();

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando referencias personales...");

                    refPersonales = await _referenciasPersonalesRepository.ObtenerReferenciasPersonales(dto);
                    scope.Complete();

                    if (refPersonales == null)
                    {
                        codigoEvento = ReferenciasPersonalesEventos.VARIAS_REFERENCIAS_PERSONALES_NO_OBTENIDAS;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    _logger.Informativo($"Referencias personales consultadas");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerReferenciasPersonales => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerReferenciasPersonales => {exc}");
                        codigoEvento = ReferenciasPersonalesEventos.VARIAS_REFERENCIAS_PERSONALES_NO_OBTENIDAS;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = refPersonales
            };
        }

        public async Task<Respuesta> EliminarReferenciaPersonal(EliminarReferenciaPersonalDto dto)
        {
            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;
            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.guid = _config.guid;

            int codigoEvento = ReferenciasPersonalesEventos.ELIMINAR_REFERENCIA_PERSONAL;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    // Obteniendo referencia personal para historico
                    ReferenciaPersonal refPersonal = await _referenciasPersonalesRepository.ObtenerReferenciaPersonal(new ObtenerReferenciaPersonalDto()
                    {
                        codigoPersona = dto.codigoPersona,
                        codigoPersonaReferida = dto.codigoPersonaReferida
                    });

                    if (refPersonal == null)
                    {
                        _logger.Informativo($"Referencia personal no existe o estado 0");
                        codigoEvento = ReferenciasPersonalesEventos.REFERENCIA_PERSONAL_NO_ELIMINADA;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    _logger.Informativo($"Eliminando referencia personal...");

                    int result = await _referenciasPersonalesRepository.EliminarReferenciaPersonal(dto);

                    if (result == 0)
                    {
                        codigoEvento = ReferenciasPersonalesEventos.REFERENCIA_PERSONAL_NO_ELIMINADA;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }
                    else
                    {
                        refPersonal.codigoUsuarioRegistro = _config.codigoUsuarioRegistra;
                        refPersonal.fechaUsuarioRegistro = DateTime.Now;
                        refPersonal.guid = _config.guid;

                        //await _referenciasPersonalesRepository.GuardarReferenciaPersonalHistorico(refPersonal);

                        if (result > 1)
                        {
                            _logger.Warning($"Referencia personal eliminada con {result} registros afectados!");
                        }
                        else
                        {
                            _logger.Informativo($"Referencia personal eliminada");
                        }
                    }

                    await _personaRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest
                    {
                        codigoPersona = dto.codigoPersona,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra
                    });

                    scope.Complete();
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"EliminarReferenciaPersonal => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"EliminarReferenciaPersonal => {exc}");
                        codigoEvento = ReferenciasPersonalesEventos.ELIMINAR_REFERENCIA_PERSONAL_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = null
            };
        }
    }
}
