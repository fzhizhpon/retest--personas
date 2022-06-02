using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.Personas;
using Personas.Core.Dtos.ReferenciasComerciales;
using Personas.Core.Entities.ReferenciasComerciales;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{

    // +900 -900
    public class ReferenciasComercialesService : IReferenciasComercialesService
    {
        protected readonly ConfiguracionApp _config;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ILogsRepository<ReferenciasComercialesService> _logger;
        protected readonly IReferenciasComercialesRepository _referenciasComercialesRepository;
        private readonly IPersonaRepository _personaRepository;

        public ReferenciasComercialesService(
            ConfiguracionApp config,
            IReferenciasComercialesRepository repository,
            IMensajesRespuestaRepository textoInfoService,
            ILogsRepository<ReferenciasComercialesService> logger,
            IPersonaRepository personaRepository
        )
        {
            _config = config;
            _logger = logger;
            _textoInfoService = textoInfoService;
            _referenciasComercialesRepository = repository;
            _personaRepository = personaRepository;
        }

        public async Task<Respuesta> GuardarReferenciaComercial(GuardarReferenciaComercialDto dto)
        {

            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;

            int codigoEvento = ReferenciasComercialesEventos.GUARDAR_REFERENCIA_COMERCIAL;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Guardando referencia comercial...");

                    dto.numeroRegistro = await _referenciasComercialesRepository.ObtenerCodigoReferenciaComercial(dto.codigoPersona);

                    int result = await _referenciasComercialesRepository.GuardarReferenciaComercial(dto);

                    if (result == 0)
                    {
                        codigoEvento = ReferenciasComercialesEventos.REFERENCIA_COMERCIAL_NO_GUARDADA;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    await _personaRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest
                    {
                        codigoPersona = dto.codigoPersona,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                    });

                    scope.Complete();

                    _logger.Informativo($"Referencia comercial guardada");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"GuardarReferenciaComercial => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarReferenciaComercial => {exc}");
                        codigoEvento = ReferenciasComercialesEventos.GUARDAR_REFERENCIA_COMERCIAL_ERROR;
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

        public async Task<Respuesta> ObtenerReferenciaComercial(ObtenerReferenciaComercialDto dto)
        {
            ReferenciaComercial refComercial = null;
            int codigoEvento = ReferenciasComercialesEventos.OBTENER_REFERENCIA_COMERCIAL;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando referencia comercial...");

                    refComercial = await _referenciasComercialesRepository.ObtenerReferenciaComercial(dto);
                    scope.Complete();

                    if (refComercial == null)
                    {
                        codigoEvento = ReferenciasComercialesEventos.REFERENCIA_COMERCIAL_NO_OBTENIDA;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);

                    }

                    _logger.Informativo($"Referencia comercial consultada");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerReferenciaComercial => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerReferenciaComercial => {exc}");
                        codigoEvento = ReferenciasComercialesEventos.OBTENER_REFERENCIA_COMERCIAL_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = refComercial
            };
        }

        public async Task<Respuesta> ObtenerReferenciasComerciales(ObtenerReferenciasComercialesDto dto)
        {
            int codigoEvento = ReferenciasComercialesEventos.OBTENER_VARIAS_REFERENCIAS_COMERCIALES;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            IList<ReferenciaComercial.ReferenciaComercialMinimo> refComerciales = new List<ReferenciaComercial.ReferenciaComercialMinimo>();

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando referencias comerciales...");

                    refComerciales = await _referenciasComercialesRepository.ObtenerReferenciasComerciales(dto);
                    scope.Complete();

                    if (refComerciales == null)
                    {
                        codigoEvento = ReferenciasComercialesEventos.VARIAS_REFERENCIAS_COMERCIALES_NO_OBTENIDAS;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);

                    }

                    _logger.Informativo($"Referencias comerciales consultados");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerReferenciasComerciales => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerReferenciasComerciales => {exc}");
                        codigoEvento = ReferenciasComercialesEventos.OBTENER_VARIAS_REFERENCIAS_COMERCIALES_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = refComerciales
            };
        }

        public async Task<Respuesta> EliminarReferenciaComercial(EliminarReferenciaComercialDto dto)
        {

            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;
            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.guid = _config.guid;

            int codigoEvento = ReferenciasComercialesEventos.ELIMINAR_REFERENCIA_COMERCIAL;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Eliminar referencia comercial...");

                    int result = await _referenciasComercialesRepository.EliminarReferenciaComercial(dto);

                    if (result == 0)
                    {
                        codigoEvento = ReferenciasComercialesEventos.REFERENCIA_COMERCIAL_NO_ELIMINADA;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }
                    else if (result > 1)
                    {
                        _logger.Warning($"Referencia comercial eliminado con {result} registros afectados!");
                    }
                    else
                    {
                        _logger.Informativo($"Referencia comercial eliminada");
                    }

                    await _personaRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest
                    {
                        codigoPersona = dto.codigoPersona,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                    });

                    scope.Complete();
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"EliminarReferenciaComercial => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"EliminarReferenciaComercial => {exc}");
                        codigoEvento = ReferenciasComercialesEventos.ELIMINAR_REFERENCIA_COMERCIAL_ERROR;
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

        public async Task<Respuesta> ActualizarReferenciaComercial(ActualizarReferenciaComercialDto dto)
        {
            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;
            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.guid = _config.guid;

            int codigoEvento = ReferenciasComercialesEventos.ACTUALIZAR_REFERENCIA_COMERCIAL;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Actualizando referencia comercial...");

                    int result = await _referenciasComercialesRepository.ActualizarReferenciaComercial(dto);

                    if (result == 0)
                    {
                        codigoEvento = ReferenciasComercialesEventos.REFERENCIAS_COMERCIAL_NO_ACTUALIZADA;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }
                    else if (result > 1)
                    {
                        _logger.Warning($"Referencia comercial actualizada con {result} registros afectados!");
                    }
                    else
                    {
                        _logger.Informativo($"Referencia comercial actualizada");
                    }

                    await _personaRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest
                    {
                        codigoPersona = dto.codigoPersona,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                    });

                    scope.Complete();
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ActualizarReferenciaComercial => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarReferenciaComercial => {exc}");
                        codigoEvento = ReferenciasComercialesEventos.ACTUALIZAR_REFERENCIA_COMERCIAL_ERROR;
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
