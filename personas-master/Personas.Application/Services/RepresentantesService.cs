using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.Personas;
using Personas.Core.Dtos.Representantes;
using Personas.Core.Entities.Representantes;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{
    public class RepresentantesService : IRepresentantesService
    {

        protected readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<RepresentantesService> _logger;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly IRepresentantesRepository _representantesRepository;
        private readonly IPersonaRepository _personaRepository;

        public RepresentantesService(
            ConfiguracionApp config,
            ILogsRepository<RepresentantesService> logger,
            IMensajesRespuestaRepository textoInfoService,
            IRepresentantesRepository representantesRepository,
            IPersonaRepository personaRepository
        )
        {
            _logger = logger;
            _config = config;
            _textoInfoService = textoInfoService;
            _representantesRepository = representantesRepository;
            _personaRepository = personaRepository;
        }

        public async Task<Respuesta> GuardarRepresentante(GuardarRepresentanteDto dto)
        {
            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;

            int codigoEvento = RepresentantesEventos.GUARDAR_REPRESENTANTE;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            IList<Representante.RepresentanteSimple> trabajos = new List<Representante.RepresentanteSimple>();

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted, Timeout = TimeSpan.FromMilliseconds(3000)},
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {

                    if(dto.codigoPersona == dto.codigoRepresentante)
                    {
                        codigoEvento = RepresentantesEventos.GUARDAR_REPRESENTANTE_ERROR_AUTO_REPRESENTACION;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                    }

                    Representante.RepresentanteSimple representante =
                        await _representantesRepository.ObtenerRepresentanteMinimo(new ObtenerRepresentanteDto()
                        {
                            codigoPersona = dto.codigoPersona,
                            codigoRepresentante = dto.codigoRepresentante,
                            estado = '0'
                        });

                    int result;

                    if (representante != null)
                    {
                        result = await _representantesRepository.ActualizarRepresentante(new ActualizarRepresentanteDto()
                        {
                            codigoPersona = dto.codigoPersona,
                            codigoRepresentante = dto.codigoRepresentante,
                            codigoTipoRepresentante = dto.codigoTipoRepresentante,
                            principal = dto.principal,
                            codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                            fechaUsuarioActualiza = DateTime.Now,
                            guid = _config.guid
                        });
                    }
                    else
                    {
                        result = await _representantesRepository.GuardarRepresentante(dto);
                    }

                    if (dto.principal == '1')
                    {
                        trabajos = await _representantesRepository.ObtenerRepresentantesPrincipales(new ObtenerRepresentantesDto()
                        {
                            codigoPersona = dto.codigoPersona
                        });

                        int trabajosCambiadosNoPrincipal = await _representantesRepository.ActualizarRepresentantesPrincipales(trabajos);
                    }

                    _logger.Informativo($"Guardando representante...");

                    if (result == 0)
                    {
                        codigoEvento = RepresentantesEventos.REPRESENTANTE_NO_GUARDADO;
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

                    _logger.Informativo($"Representante guardado");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"GuardarRepresentante => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarRepresentante => {exc}");
                        codigoEvento = RepresentantesEventos.GUARDAR_REPRESENTANTE_ERROR;
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

        public async Task<Respuesta> ActualizarRepresentante(ActualizarRepresentanteDto dto)
        {
            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;
            dto.guid = _config.guid;

            int codigoEvento = RepresentantesEventos.ACTUALIZAR_REPRESENTANTE;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            IList<Representante.RepresentanteSimple> trabajos = new List<Representante.RepresentanteSimple>();

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Actualizando representante...");

                    int result = await _representantesRepository.ActualizarRepresentante(dto);

                    if (result == 0)
                    {
                        Console.WriteLine(result);
                        codigoEvento = RepresentantesEventos.REPRESENTANTE_NO_ACTUALIZADO;
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

                    _logger.Informativo($"Representante actualizado");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ActualizarRepresentante => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarRepresentante => {exc}");
                        codigoEvento = RepresentantesEventos.ACTUALIZAR_REPRESENTANTE_ERROR;
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

        public async Task<Respuesta> EliminarRepresentante(EliminarRepresentanteDto dto)
        {
            int codigoEvento = RepresentantesEventos.ELIMINAR_REPRESENTANTE;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Eliminando representante...");

                    int result = await _representantesRepository.EliminarRepresentante(dto);

                    if (result == 0)
                    {
                        codigoEvento = RepresentantesEventos.REPRESENTANTE_NO_ELIMINADO;
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

                    _logger.Informativo($"Representante eliminado");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"EliminarRepresentante => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"EliminarRepresentante => {exc}");
                        codigoEvento = RepresentantesEventos.ELIMINAR_REPRESENTANTE_ERROR;
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

        public async Task<Respuesta> ObtenerRepresentante(int codigoPersona, int codigoRepresentante)
        {
            ObtenerRepresentanteDto dto = new ObtenerRepresentanteDto()
            {
                codigoPersona = codigoPersona,
                codigoRepresentante = codigoRepresentante
            };
            int codigoEvento = RepresentantesEventos.OBTENER_REPRESENTANTE;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            Representante.RepresentanteJoin representante = null;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando representante...");

                    representante = await _representantesRepository.ObtenerRepresentante(dto);
                    scope.Complete();

                    if (representante == null)
                    {
                        codigoEvento = RepresentantesEventos.REPRESENTANTE_NO_OBTENIDO;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    _logger.Informativo($"Representante consultado");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerRepresentante => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerRepresentante => {exc}");
                        codigoEvento = RepresentantesEventos.OBTENER_REPRESENTANTE_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = representante
            };
        }

        public async Task<Respuesta> ObtenerRepresentantes(int codigoPersona)
        {
            ObtenerRepresentantesDto dto = new ObtenerRepresentantesDto()
            {
                codigoPersona = codigoPersona
            };
            int codigoEvento = RepresentantesEventos.GUARDAR_REPRESENTANTE;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            IList<Representante.RepresentanteJoin> representantes = new List<Representante.RepresentanteJoin>();

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando representantes...");

                    representantes = await _representantesRepository.ObtenerRepresentantes(dto);
                    scope.Complete();

                    if (representantes == null)
                    {
                        codigoEvento = RepresentantesEventos.VARIOS_REPRESENTANTES_NO_OBTENIDOS;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    _logger.Informativo($"Representantes consultados");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"GuardarRepresentante => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarRepresentante => {exc}");
                        codigoEvento = RepresentantesEventos.OBTENER_VARIOS_REPRESENTANTES_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = representantes
            };
        }
    }
}
