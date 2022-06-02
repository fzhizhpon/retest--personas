using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.TelefonosFijos;
using Personas.Core.Entities.TelefonosFijos;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{
    public class TelefonosFijosService : ITelefonosFijosService
    {

        protected readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<TelefonosFijosService> _logger;
        protected readonly ITelefonosFijosRepository _teleFijoRepository;
        protected readonly IMensajesRespuestaRepository _textoInfoService;


        public TelefonosFijosService(
            ConfiguracionApp config,
            ILogsRepository<TelefonosFijosService> logger,
            ITelefonosFijosRepository teleFijoRepository,
            IMensajesRespuestaRepository textoInfoService
        )
        {
            _logger = logger;
            _config = config;
            _textoInfoService = textoInfoService;
            _teleFijoRepository = teleFijoRepository;
        }

        public async Task<Respuesta> GuardarTelefonoFijo(GuardarTelefonoFijoDto dto)
        {
            int codigoEvento = TelefonosFijosEventos.GUARDAR_TELEFONO_FIJO;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;
                    dto.fechaUsuarioActualiza = DateTime.Now;

                    int ultimoRegistro = await _teleFijoRepository.GenerarNumeroRegistro(dto.codigoPersona);
                    dto.numeroRegistro = ultimoRegistro + 1;

                    _logger.Informativo($"Guardando telefono fijo...");

                    int result = await _teleFijoRepository.GuardarTelefonoFijo(dto);

                    if (result == 0)
                    {
                        codigoEvento = TelefonosFijosEventos.TELEFONO_FIJO_NO_GUARDADO;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    scope.Complete();

                    _logger.Informativo($"Telefono fijo guardado");

                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"GuardarTelefonoFijo => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarTelefonoFijo => {exc}");
                        codigoEvento = TelefonosFijosEventos.GUARDAR_TELEFONO_FIJO_ERROR;
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

        public async Task<Respuesta> ActualizarTelefonoFijo(ActualizarTelefonoFijoDto dto)
        {
            int codigoEvento = TelefonosFijosEventos.ACTUALIZAR_TELEFONO_FIJO;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Actualizando telefono fijo...");

                    int result = await _teleFijoRepository.ActualizarTelefonoFijo(dto);

                    if (result == 0)
                    {
                        codigoEvento = TelefonosFijosEventos.TELEFONO_FIJO_NO_ACTUALIZADO;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    scope.Complete();

                    _logger.Informativo($"Telefono fijo actualizado");

                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ActualizarTelefonoFijo => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarTelefonoFijo => {exc}");
                        codigoEvento = TelefonosFijosEventos.ACTUALIZAR_TELEFONO_FIJO_ERROR;
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

        public async Task<Respuesta> EliminarTelefonoFijo(EliminarTelefonoFijoDto dto)
        {
            int codigoEvento = TelefonosFijosEventos.ELIMINAR_TELEFONO_FIJO;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Eliminando telefono fijo...");

                    int result = await _teleFijoRepository.EliminarTelefonoFijo(dto);

                    if (result == 0)
                    {
                        codigoEvento = TelefonosFijosEventos.TELEFONO_FIJO_NO_ELIMINADO;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    scope.Complete();

                    _logger.Informativo($"Telefono fijo eliminado");

                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"EliminarTelefonoFijo => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"EliminarTelefonoFijo => {exc}");
                        codigoEvento = TelefonosFijosEventos.ELIMINAR_TELEFONO_FIJO_ERROR;
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

        public async Task<Respuesta> ObtenerTelefonosFijos(ObtenerTelefonosFijosDto dto)
        {
            int codigoEvento = TelefonosFijosEventos.OBTENER_VARIOS_TELEFONO_FIJOS;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            IList<TelefonoFijo.TelefonoFijoMinimo> telefonosFijos = null;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando telefonos fijos...");

                    telefonosFijos = await _teleFijoRepository.ObtenerTelefonosFijos(dto);

                    if (telefonosFijos == null || telefonosFijos.Count == 0)
                    {
                        codigoEvento = TelefonosFijosEventos.VARIOS_TELEFONO_FIJOS_NO_OBTENIDOS;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    scope.Complete();

                    _logger.Informativo($"Telefonos fijos consultados");

                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerTelefonosFijos => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerTelefonosFijos => {exc}");
                        codigoEvento = TelefonosFijosEventos.OBTENER_VARIOS_TELEFONO_FIJOS_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = telefonosFijos
            };
        }

        public async Task<Respuesta> ObtenerTelefonoFijo(ObtenerTelefonoFijoDto dto)
        {
            int codigoEvento = TelefonosFijosEventos.OBTENER_TELEFONO_FIJO;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            TelefonoFijo.TelefonoFijoFull telefonoFijo = null;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando telefono fijo...");

                    telefonoFijo = await _teleFijoRepository.ObtenerTelefonoFijo(dto);

                    if (telefonoFijo == null)
                    {
                        codigoEvento = TelefonosFijosEventos.TELEFONO_FIJO_NO_OBTENIDO;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    scope.Complete();

                    _logger.Informativo($"Telefono fijo consultado");

                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerTelefonoFijo => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerTelefonoFijo => {exc}");
                        codigoEvento = TelefonosFijosEventos.OBTENER_TELEFONO_FIJO_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = telefonoFijo
            };
        }


    }
}
