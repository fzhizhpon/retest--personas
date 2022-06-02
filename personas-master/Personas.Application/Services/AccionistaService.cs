using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.Accionistas;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{
    public class AccionistaService : IAccionistaService
    {
        protected readonly IAccionistaRepository _repository;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<AccionistaService> _logger;

        public AccionistaService(IAccionistaRepository repository,
            IMensajesRespuestaRepository textoInfoService,
            ILogsRepository<AccionistaService> logger,
            ConfiguracionApp config)
        {
            _repository = repository;
            _textoInfoService = textoInfoService;
            _config = config;
            _logger = logger;
        }

        public async Task<Respuesta> GuardarAccionistas(List<GuardarAccionistaDto> accionistasDto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                int result = 0;
                int codigoEvento = AccionistasEventos.GUARDAR_LISTA_ACCIONISTAS_OK;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo($"Guardando accionistas");

                    foreach (var item in accionistasDto)
                    {
                        item.codigoUsuarioRegistra = _config.codigoUsuarioRegistra;
                        item.fechaUsuarioRegistra = DateTime.Now;
                    }

                    result = await _repository.GuardarAccionistas(accionistasDto);

                    if (result == 0)
                    {
                        throw new ExcepcionOperativa(AccionistasEventos.GUARDAR_LISTA_ACCIONISTAS_NO_GUARDADO);
                    }

                    scope.Complete();

                    _logger.Informativo($"Accionistas guardados");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"GuardarAccionistas=> {exo.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarAccionistas=> {ex}");
                        codigoEvento = AccionistasEventos.GUARDAR_LISTA_ACCIONISTAS_ERROR;
                    }
                }

                _logger.Informativo($"GuardarAccionistas => {codigoEvento}");

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

        public async Task<Respuesta> ActualizarAccionista(ActualizarAccionistaDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                int result = 0;
                int codigoEvento = AccionistasEventos.GUARDAR_LISTA_ACCIONISTAS_OK;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo($"Actualizando accionista");

                    dto.codigoUsuarioRegistra = _config.codigoUsuarioRegistra;
                    dto.fechaUsuarioRegistra = DateTime.Now;

                    result = await _repository.ActualizarAccionista(dto);

                    if (result == 0)
                    {
                        throw new ExcepcionOperativa(AccionistasEventos.GUARDAR_LISTA_ACCIONISTAS_NO_GUARDADO);
                    }

                    scope.Complete();

                    _logger.Informativo($"Accionista actualizado");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ActualizarAccionista=> {exo.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarAccionista=> {ex}");
                        codigoEvento = AccionistasEventos.ACTUALIZAR_ACCIONISTA_ERROR;
                    }
                }

                _logger.Informativo($"ActualizarAccionista => {codigoEvento}");

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

        public async Task<Respuesta> ObtenerAccionistas(AccionistaRequest dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int codigoEvento = AccionistasEventos.OBTENER_ACCIONISTA_OK;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
                IEnumerable<AccionistaResponse> accionistas = null;

                try
                {
                    _logger.Informativo($"Obteniendo accionistas");

                    accionistas = await _repository.ObtenerAccionistas(dto);

                    scope.Complete();

                    _logger.Informativo($"Accionistas obtenidos");
                }
                catch (Exception ex)
                {
                    if (ex is ExcepcionOperativa)
                    {
                        _logger.Error($"ObtenerAccionistas => {ex.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerAccionistas => {ex}");
                        codigoEvento = AccionistasEventos.OBTENER_ACCIONISTA_ERROR;
                    }

                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                }

                _logger.Informativo($"ObtenerAccionistas => {codigoEvento}");

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                   _config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta()
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = accionistas
                };
            }
        }
    }
}
