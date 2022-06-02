using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.EstadosFinancieros;
using Personas.Core.Dtos.Personas;
using Personas.Core.Entities.EstadosFinancieros;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{
    public class EstadosFinancierosService : IEstadosFinancierosService
    {

        protected readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<EstadosFinancierosService> _logger;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly IEstadosFinancierosRepository _estadoFinRepository;
        private readonly IPersonaRepository _personaRepository;

        public EstadosFinancierosService(
            ConfiguracionApp config,
            ILogsRepository<EstadosFinancierosService> logger,
            IMensajesRespuestaRepository textoInfoService,
            IEstadosFinancierosRepository estadoFinRepository,
            IPersonaRepository personaRepository
        )
        {
            _logger = logger;
            _config = config;
            _textoInfoService = textoInfoService;
            _estadoFinRepository = estadoFinRepository;
            _personaRepository = personaRepository;
        }

        public async Task<Respuesta> GuardarEstadosFinancieros(GuardarEstadoFinancieroDto dto)
        {
            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;

            int codigoEvento = EstadosFinancierosEventos.GUARDAR_ESTADO_FINANCIERO;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Guardando estado financiero...");

                    int result = await _estadoFinRepository.GuardarEstadosFinancieros(dto);

                    if (result == 0)
                    {
                        codigoEvento = EstadosFinancierosEventos.ESTADO_FINANCIERO_NO_GUARDADO;
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

                    _logger.Informativo($"Estado financiero guardado");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"GuardarEstadosFinancieros => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarEstadosFinancieros => {exc}");
                        codigoEvento = EstadosFinancierosEventos.GUARDAR_ESTADO_FINANCIERO_ERROR;
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

        public async Task<Respuesta> ActualizarEstadosFinancieros(ActualizarEstadoFinancieroDto dto)
        {
            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;
            dto.guid = _config.guid;

            int codigoEvento = EstadosFinancierosEventos.ACTUALIZAR_ESTADO_FINANCIERO;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Actualizando estado financiero...");
                    int result = await _estadoFinRepository.ActualizarEstadosFinancieros(dto);

                    if (result == 0)
                    {
                        codigoEvento = EstadosFinancierosEventos.ESTADO_FINANCIERO_NO_ACTUALIZADO;
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

                    _logger.Informativo($"Estado financiero actualizado");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ActualizarEstadosFinancieros => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarEstadosFinancieros => {exc}");
                        codigoEvento = EstadosFinancierosEventos.ACTUALIZAR_ESTADO_FINANCIERO_ERROR;
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

        public async Task<Respuesta> ObtenerCuentasEstadosFinancieros(ObtenerCuentasEstadoFinancieroDto dto)
        {
            int codigoEvento = EstadosFinancierosEventos.OBTENER_CUENTAS_ESTADO_FINANCIERO;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            List<EstadoFinanciero> cuentasEstFin = null;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando cuentas estado financiero...");

                    cuentasEstFin = await _estadoFinRepository.ObtenerCuentasEstadosFinancieros(dto);
                    scope.Complete();

                    cuentasEstFin.ForEach(async cuenta =>
                    {
                        if (!String.IsNullOrEmpty(cuenta.query))
                        {
                            cuenta.valor = await _estadoFinRepository.ObtenerValorCuentaPorQuery(cuenta.query, dto.codigoPersona);
                            cuenta.recursoExterno = true;
                        }
                    });

                    if (cuentasEstFin == null)
                    {
                        codigoEvento = EstadosFinancierosEventos.CUENTAS_ESTADO_FINANCIERO_NO_OBTENIDAS;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    _logger.Informativo($"Cuentas estado financiero consultadas");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerCuentasEstadosFinancieros => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerCuentasEstadosFinancieros => {exc}");
                        codigoEvento = EstadosFinancierosEventos.OBTENER_CUENTAS_ESTADO_FINANCIERO_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = cuentasEstFin
            };
        }
    }
}
