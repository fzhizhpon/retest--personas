using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.Personas;
using Personas.Core.Dtos.Trabajos;
using Personas.Core.Entities.Trabajos;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{
    // Rango de enteros para identificación de eventos +400 -400

    public class TrabajosService : ITrabajosService
    {
        protected readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<TrabajosService> _logger;
        protected readonly ITrabajosRepository _trabajosRepository;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        private readonly IPersonaRepository _personaRepository;

        public TrabajosService(
            ConfiguracionApp config,
            ILogsRepository<TrabajosService> logger,
            ITrabajosRepository serviceTrabajosRepository,
            IMensajesRespuestaRepository textoInfoService,
            IPersonaRepository personaRepository
        )
        {
            _logger = logger;
            _config = config;
            _textoInfoService = textoInfoService;
            _trabajosRepository = serviceTrabajosRepository;
            _personaRepository = personaRepository;
        }

        public async Task<Respuesta> GuardarTrabajo(GuardarTrabajoDto dto)
        {
            int codigoEvento = TrabajosEventos.GUARDAR_TRABAJO;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Guardando trabajo...");

                    int codigoTrabajo = await _trabajosRepository.ObtenerCodigoTrabajo(dto.codigoPersona);
                    Console.WriteLine(codigoTrabajo + 1);

                    int result = await _trabajosRepository.GuardarTrabajo(codigoTrabajo + 1, dto);

                    if (result == 0)
                    {
                        codigoEvento = TrabajosEventos.TRABAJO_NO_GUARDADO;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    await _personaRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest {
                        codigoPersona = dto.codigoPersona,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra
                    });

                    scope.Complete();

                    _logger.Informativo($"Trabajo guardado");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"GuardarTrabajo => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarTrabajo => {exc}");
                        codigoEvento = TrabajosEventos.GUARDAR_TRABAJO_ERROR;
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

        public async Task<Respuesta> ObtenerTrabajo(ObtenerTrabajoDto dto)
        {
            Trabajo trabajo = null;
            int codigoEvento = TrabajosEventos.OBTENER_TRABAJO;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando trabajo...");

                    trabajo = await _trabajosRepository.ObtenerTrabajo(dto);

                    if (trabajo == null)
                    {
                        codigoEvento = TrabajosEventos.TRABAJO_NO_OBTENIDO;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    scope.Complete();

                    _logger.Informativo($"Trabajo consultado");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerTrabajo => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerTrabajo => {exc}");
                        codigoEvento = TrabajosEventos.OBTENER_TRABAJO_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = trabajo
            };
        }

        public async Task<Respuesta> ObtenerTrabajos(ObtenerTrabajosDto dto)
        {
            int codigoEvento = TrabajosEventos.OBTENER_VARIOS_TRABAJOS;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            IList<Trabajo.TrabajoMinimo> trabajos = new List<Trabajo.TrabajoMinimo>();

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando trabajos...");

                    trabajos = await _trabajosRepository.ObtenerTrabajos(dto);
                    scope.Complete();

                    if (trabajos == null)
                    {
                        codigoEvento = TrabajosEventos.VARIOS_TRABAJOS_NO_OBTENIDOS;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    _logger.Informativo($"Trabajos consultados");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerTrabajos => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerTrabajos => {exc}");
                        codigoEvento = TrabajosEventos.OBTENER_VARIOS_TRABAJOS_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = trabajos
            };
        }

        public async Task<Respuesta> EliminarTrabajo(EliminarTrabajoDto dto)
        {
            dto.codigoUsuarioRegistro = _config.codigoUsuarioRegistra;
            dto.fechaUsuarioRegistro = DateTime.Now;
            dto.guid = _config.guid;

            int codigoEvento = TrabajosEventos.ELIMINAR_TRABAJO;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {

                    // Consultar trabajo para guardar en repoitorio
                    Trabajo trabajo = await _trabajosRepository.ObtenerTrabajo(new ObtenerTrabajoDto()
                    {
                        codigoPersona = dto.codigoPersona,
                        codigoTrabajo = dto.codigoTrabajo
                    });

                    _logger.Informativo($"Eliminar trabajo...");

                    int result = await _trabajosRepository.EliminarTrabajo(dto);

                    if (result == 0)
                    {
                        codigoEvento = TrabajosEventos.TRABAJO_NO_ELIMINADO;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }
                    else
                    {
                        trabajo.codigoUsuarioRegistro = _config.codigoUsuarioRegistra;
                        trabajo.fechaUsuarioRegistro = DateTime.Now;
                        trabajo.guid = _config.guid;

                        //await _trabajosRepository.GuardarTrabajoHistorico(trabajo);

                        if (result > 1)
                        {
                            _logger.Warning($"Trabajo eliminado con {result} registros afectados!");
                        }
                        else
                        {
                            _logger.Informativo($"Trabajo eliminado");
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
                        _logger.Error($"GuardarTrabajo => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarTrabajo => {exc}");
                        codigoEvento = TrabajosEventos.ELIMINAR_TRABAJO_ERROR;
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

        public async Task<Respuesta> ActualizarTrabajo(ActualizarTrabajoDto dto)
        {
            int codigoEvento = TrabajosEventos.ACTUALIZAR_TRABAJO;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    // Consultar trabajo para guardar en repoitorio
                    Trabajo trabajo = await _trabajosRepository.ObtenerTrabajo(new ObtenerTrabajoDto()
                    {
                        codigoPersona = dto.codigoPersona,
                        codigoTrabajo = dto.codigoTrabajo
                    });

                    if (trabajo == null)
                    {
                        throw new ExcepcionOperativa(TrabajosEventos.OBTENER_TRABAJO_ERROR);
                    }

                    _logger.Informativo($"Actualizando trabajo...");

                    int result = await _trabajosRepository.ActualizarTrabajo(dto);

                    if (result == 0)
                    {
                        codigoEvento = TrabajosEventos.TRABAJO_NO_ACTUALIZADO; // No se guardo el trabajo
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }
                    else
                    {
                        trabajo.codigoUsuarioRegistro = _config.codigoUsuarioRegistra;
                        trabajo.fechaUsuarioRegistro = DateTime.Now;
                        trabajo.guid = _config.guid;

                        //await _trabajosRepository.GuardarTrabajoHistorico(trabajo);

                        if (result > 1)
                        {
                            _logger.Warning($"Trabajo actualizado con {result} registros afectados!");
                        }
                        else
                        {
                            _logger.Informativo($"Trabajo actualizado");
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
                        _logger.Error($"ActualizarTrabajo => {excOperativa.codigoEvento} - {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarTrabajo => {exc}");
                        codigoEvento = TrabajosEventos.ACTUALIZAR_TRABAJO_ERROR;
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
