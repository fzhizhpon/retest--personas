using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.Personas;
using Personas.Core.Dtos.ReferenciasFinancieras;
using Personas.Core.Entities.ReferenciasFinancieras;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{

    // +800 -800

    public class ReferenciasFinancierasService : IReferenciasFinancierasService
    {
        protected readonly ConfiguracionApp _config;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ILogsRepository<ReferenciasFinancierasService> _logger;
        protected readonly IReferenciasFinancierasRepository _referenciasFinancierasRepository;
        private readonly IPersonaRepository _personaRepository;

        public ReferenciasFinancierasService(
            ConfiguracionApp config,
            ILogsRepository<ReferenciasFinancierasService> logger,
            IMensajesRespuestaRepository textoInfoService,
            IReferenciasFinancierasRepository serviceReferenciasFinancierasRepository,
            IPersonaRepository personaRepository
            )
        {
            _logger = logger;
            _config = config;
            _textoInfoService = textoInfoService;
            _referenciasFinancierasRepository = serviceReferenciasFinancierasRepository;
            _personaRepository = personaRepository;
        }

        public async Task<Respuesta> GuardarReferenciaFinanciera(GuardarReferenciaFinancieraDto dto)
        {
            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;

            int codigoEvento = ReferenciasFinancierasEventos.GUARDAR_REFERENCIA_FINANCIERA;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Guardando referencia financiera...");

                    int codigoReferencia = await _referenciasFinancierasRepository.ObtenerCodigoReferenciaFinanciera(dto.codigoPersona);
                    dto.numeroRegistro = codigoReferencia;

                    int result = await _referenciasFinancierasRepository.GuardarReferenciaFinanciera(dto);

                    if (result == 0)
                    {
                        codigoEvento = ReferenciasFinancierasEventos.REFERENCIA_FINANCIERA_NO_GUARDADA;
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

                    _logger.Informativo($"Referencia financiera guardada");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"GuardarReferenciaFinanciera => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarReferenciaFinanciera => {exc}");
                        codigoEvento = ReferenciasFinancierasEventos.GUARDAR_REFERENCIA_FINANCIERA_ERROR;
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

        public async Task<Respuesta> ObtenerReferenciaFinanciera(ObtenerReferenciaFinancieraDto dto)
        {

            ReferenciaFinanciera refFinanciera = null;
            int codigoEvento = ReferenciasFinancierasEventos.OBTENER_REFERENCIA_FINANCIERA;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando referencia financiera...");

                    refFinanciera = await _referenciasFinancierasRepository.ObtenerReferenciaFinanciera(dto);

                    if (refFinanciera == null)

                    {
                        codigoEvento = ReferenciasFinancierasEventos.REFERENCIA_FINANCIERA_NO_OBTENIDA;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    scope.Complete();

                    _logger.Informativo($"Referencia financiera no consultada");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerReferenciaFinanciera => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerReferenciaFinanciera => {exc}");
                        codigoEvento = ReferenciasFinancierasEventos.OBTENER_REFERENCIA_FINANCIERA_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = refFinanciera
            };
        }

        public async Task<Respuesta> ObtenerReferenciasFinancieras(ObtenerReferenciasFinancierasDto dto)
        {
            int codigoEvento = ReferenciasFinancierasEventos.OBTENER_VARIAS_REFERENCIAS_FINANCIERAS;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            IList<ReferenciaFinanciera> refFinancieras = new List<ReferenciaFinanciera>();

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando varias referencias financieras...");

                    refFinancieras = await _referenciasFinancierasRepository.ObtenerReferenciasFinancieras(dto);
                    scope.Complete();

                    if (refFinancieras == null)
                    {
                        codigoEvento = ReferenciasFinancierasEventos.VARIAS_REFERENCIAS_FINANCIERAS_NO_OBTENIDAS;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    _logger.Informativo($"Varias referencias financieras consultadas");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerReferenciasFinancieras => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerReferenciasFinancieras => {exc}");
                        codigoEvento = ReferenciasFinancierasEventos.OBTENER_VARIAS_REFERENCIAS_FINANCIERAS_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = refFinancieras
            };
        }

        public async Task<Respuesta> EliminarReferenciaFinanciera(EliminarReferenciaFinancieraDto dto)
        {
            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;
            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.guid = _config.guid;

            int codigoEvento = ReferenciasFinancierasEventos.ELIMINAR_REFERENCIA_FINANCIERA;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    // Consulto referencia para guardar en historico
                    ReferenciaFinanciera refFinanciera = await _referenciasFinancierasRepository
                        .ObtenerReferenciaFinanciera(new ObtenerReferenciaFinancieraDto()
                        {
                            codigoPersona = dto.codigoPersona,
                            numeroRegistro = dto.numeroRegistro
                        });

                    _logger.Informativo($"Eliminar trabajo...");

                    int result = await _referenciasFinancierasRepository.EliminarReferenciaFinanciera(dto);

                    if (result == 0)
                    {
                        codigoEvento = ReferenciasFinancierasEventos.REFERENCIA_FINANCIERA_NO_ELIMINADA;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }
                    else
                    {
                        refFinanciera.codigoUsuarioRegistra = _config.codigoUsuarioRegistra;
                        refFinanciera.fechaUsuarioRegistra = DateTime.Now;
                        refFinanciera.guid = _config.guid;

                        //await _referenciasFinancierasRepository.GuardarReferenciaFinancieraHistorico(refFinanciera);

                        if (result > 1)
                        {
                            _logger.Warning($"Referencia financiera eliminada con {result} registros afectados!");
                        }
                        else
                        {
                            _logger.Informativo($"Referencia financiera eliminada");
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
                        _logger.Error($"EliminarReferenciaFinanciera => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"EliminarReferenciaFinanciera => {exc}");
                        codigoEvento = ReferenciasFinancierasEventos.ELIMINAR_REFERENCIA_FINANCIERA_ERROR;
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

        public async Task<Respuesta> ActualizarReferenciaFinanciera(ActualizarReferenciaFinancieraDto dto)
        {
            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;
            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.guid = _config.guid;

            int codigoEvento = ReferenciasFinancierasEventos.ACTUALIZAR_REFERENCIA_FINANCIERA;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    // Consulto referencia para guardar en historico
                    ReferenciaFinanciera refFinanciera = await _referenciasFinancierasRepository
                        .ObtenerReferenciaFinanciera(new ObtenerReferenciaFinancieraDto()
                        {
                            codigoPersona = dto.codigoPersona,
                            numeroRegistro = dto.numeroRegistro
                        });

                    _logger.Informativo($"Actualizando referencia financiera...");

                    int result = await _referenciasFinancierasRepository.ActualizarReferenciaFinanciera(dto);

                    if (result == 0)
                    {
                        codigoEvento = ReferenciasFinancierasEventos.REFERENCIAS_FINANCIERA_NO_ACTUALIZADA;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }
                    else
                    {
                        refFinanciera.codigoUsuarioRegistra = _config.codigoUsuarioRegistra;
                        refFinanciera.fechaUsuarioRegistra = DateTime.Now;
                        refFinanciera.guid = _config.guid;

                        //await _referenciasFinancierasRepository.GuardarReferenciaFinancieraHistorico(refFinanciera);

                        if (result > 1)
                        {
                            _logger.Warning($"Referencia financiera actualizada con {result} registros afectados!");
                        }
                        else
                        {
                            _logger.Informativo($"Referencia financiera actualizada");
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
                        _logger.Error($"ActualizarReferenciaFinanciera => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarReferenciaFinanciera => {exc}");
                        codigoEvento = ReferenciasFinancierasEventos.ACTUALIZAR_REFERENCIA_FINANCIERA_ERROR;
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
