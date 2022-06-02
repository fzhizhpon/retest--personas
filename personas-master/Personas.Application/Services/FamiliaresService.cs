using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.Familiares;
using Personas.Core.Dtos.Personas;
using Personas.Core.Entities.Familiares;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{
    public class FamiliaresService : IFamiliaresService
    {

        private readonly ConfiguracionApp _config;
        private readonly ILogsRepository<RepresentantesService> _logger;
        private readonly IMensajesRespuestaRepository _textoInfoService;
        private readonly IFamiliaresRepository _familiaresRepository;
        //private readonly IHistoricosRepository<Familiar> _historicosRepository;
        private readonly IPersonaRepository _personaRepository;

        public FamiliaresService(
            ConfiguracionApp config,
            ILogsRepository<RepresentantesService> logger,
            IMensajesRespuestaRepository textoInfoService,
            IFamiliaresRepository familiaresRepository,
            IPersonaRepository personaRepository
        //IHistoricosRepository<Familiar> historicosRepository
        )
        {
            _logger = logger;
            _config = config;
            _textoInfoService = textoInfoService;
            _familiaresRepository = familiaresRepository;
            _personaRepository = personaRepository;
            //_historicosRepository = historicosRepository;
        }

        public async Task<Respuesta> GuardarFamiliar(GuardarFamiliarDto dto)
        {
            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;

            int codigoEvento = FamiliaresEventos.GUARDAR_FAMILIAR;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    if (dto.codigoPersona == dto.codigoPersonaFamiliar) throw new ExcepcionOperativa(FamiliaresEventos.COD_PERSONA_IGUAL_FAMILIAR);

                    DateTime fechaActual = DateTime.Now;

                    Familiar familiar = await _familiaresRepository.ObtenerFamiliar(new ObtenerFamiliarDto()
                    {
                        codigoPersona = dto.codigoPersona,
                        codigoPersonaFamiliar = dto.codigoPersonaFamiliar
                    });

                    int result;

                    if (familiar != null)
                    {
                        result = await _familiaresRepository.ActualizarFamiliar(new ActualizarFamiliarDto()
                        {
                            codigoPersonaFamiliar = dto.codigoPersonaFamiliar,
                            codigoPersona = dto.codigoPersona,
                            codigoParentesco = dto.codigoParentesco,
                            esCargaFamiliar = dto.esCargaFamiliar,
                            observacion = dto.observacion,
                            codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                            fechaUsuarioActualiza = fechaActual,
                            guid = _config.guid
                        });
                    }
                    else
                    {
                        _logger.Informativo($"Guardando familiar...");
                        result = await _familiaresRepository.GuardarFamiliar(dto);
                    }

                    await _personaRepository.ColocarFechaUltimaActualizacion(new Core.Dtos.Personas.UltActPersonaRequest
                    {
                        codigoPersona = dto.codigoPersona,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                        fechaUsuarioActualiza = fechaActual
                    });

                    if (result == 0)
                    {
                        codigoEvento = FamiliaresEventos.FAMILIAR_NO_GUARDADO;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    scope.Complete();

                    _logger.Informativo($"Familiar guardado");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"GuardarFamiliar => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarFamiliar => {exc}");
                        codigoEvento = FamiliaresEventos.GUARDAR_FAMILIAR_ERROR;
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

        public async Task<Respuesta> ActualizarFamiliar(ActualizarFamiliarDto dto)
        {
            dto.fechaUsuarioActualiza = DateTime.Now;
            dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;
            dto.guid = _config.guid;

            int codigoEvento = FamiliaresEventos.ACTUALIZAR_FAMILIAR;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    if (dto.codigoPersona == dto.codigoPersonaFamiliar) throw new ExcepcionOperativa(FamiliaresEventos.COD_PERSONA_IGUAL_FAMILIAR);

                    DateTime fechaActual = DateTime.Now;
                    dto.fechaUsuarioActualiza = fechaActual;
                    dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;

                    _logger.Informativo($"Actualizando familiar...");

                    int result = await _familiaresRepository.ActualizarFamiliar(dto);

                    if (result == 0)
                    {
                        codigoEvento = FamiliaresEventos.FAMILIAR_NO_ACTUALIZADO;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    await _personaRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest
                    {
                        codigoPersona = dto.codigoPersona,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                        fechaUsuarioActualiza = fechaActual
                    });

                    scope.Complete();

                    _logger.Informativo($"Familiar actualizado");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ActualizarFamiliar => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarFamiliar => {exc}");
                        codigoEvento = FamiliaresEventos.ACTUALIZAR_FAMILIAR;
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

        public async Task<Respuesta> ObtenerFamiliares(ObtenerFamiliaresDto dto)
        {
            int codigoEvento = RepresentantesEventos.GUARDAR_REPRESENTANTE;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            IList<Familiar.FamiliarJoinMinimo> familiares = new List<Familiar.FamiliarJoinMinimo>();

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando familiares...");

                    familiares = await _familiaresRepository.ObtenerFamiliaresJoinMinimo(dto);

                    if (familiares == null)
                    {
                        codigoEvento = FamiliaresEventos.VARIOS_FAMILIARES_NO_OBTENIDOS;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    scope.Complete();

                    _logger.Informativo($"Familiares consultados");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerFamiliaresJoinMinimo => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerFamiliaresJoinMinimo => {exc}");
                        codigoEvento = FamiliaresEventos.OBTENER_VARIOS_FAMILIARES_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = familiares
            };
        }

        public async Task<Respuesta> EliminarFamiliar(EliminarFamiliarDto dto)
        {
            int codigoEvento = FamiliaresEventos.ELIMINAR_FAMILIAR;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Eliminando familiar...");

                    int result = await _familiaresRepository.EliminarFamiliar(dto);

                    if (result == 0)
                    {
                        codigoEvento = FamiliaresEventos.FAMILIAR_NO_ELIMINADO;
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
                        _logger.Error($"EliminarFamiliar => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"EliminarFamiliar => {exc}");
                        codigoEvento = FamiliaresEventos.ELIMINAR_FAMILIAR_ERROR;
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

        public async Task<Respuesta> ObtenerFamiliar(ObtenerFamiliarDto dto)
        {
            int codigoEvento = FamiliaresEventos.OBTENER_FAMILIAR;
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
            Familiar.FamiliarJoinFull familiar = null;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo($"Consultando familiar...");

                    familiar = await _familiaresRepository.ObtenerFamiliarJoinFull(dto);

                    if (familiar == null)
                    {
                        codigoEvento = FamiliaresEventos.FAMILIAR_NO_OBTENIDO;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    scope.Complete();

                    _logger.Informativo($"Familiar consultado");
                }
                catch (Exception exc)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (exc is ExcepcionOperativa excOperativa)
                    {
                        _logger.Error($"ObtenerFamiliarJoinFull => {excOperativa.InnerException}");
                        codigoEvento = excOperativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerFamiliarJoinFull => {exc}");
                        codigoEvento = FamiliaresEventos.OBTENER_FAMILIAR_ERROR;
                    }
                }
            }

            string mensaje = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = mensaje,
                resultado = familiar
            };
        }
    }
}
