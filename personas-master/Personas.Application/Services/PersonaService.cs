using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.Personas;
using Personas.Core.Entities.Personas;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;
using VimaCoop.Validadores;

namespace Personas.Application.Services
{
    public class PersonaService : IPersonaService
    {
        protected readonly IPersonaRepository _personasRepository;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<PersonaService> _logger;

        public PersonaService(IPersonaRepository repository,
            IMensajesRespuestaRepository textoInfoService,
            ILogsRepository<PersonaService> logger,
            ConfiguracionApp config)
        {
            _personasRepository = repository;
            _textoInfoService = textoInfoService;
            _config = config;
            _logger = logger;
        }

        public async Task<Respuesta> ActualizarPersona(ActualizarPersonaDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                int result = 0;
                int codigoEvento = PersonasEventos.ACTUALIZAR_PERSONA_OK;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo($"Actualizando persona");

                    dto.codigoUsuarioRegistra = _config.codigoUsuarioRegistra;
                    dto.fechaUsuarioRegistra = DateTime.Now;

                    if (dto.codigoTipoIdentificacion == 1) // Cedula
                    {
                        if (!dto.numeroIdentificacion.EsCedulaValida()) throw new ExcepcionOperativa(PersonasEventos.ERROR_CEDULA_INVALIDA);
                    }

                    if (dto.codigoTipoIdentificacion == 2) // RUC
                    {
                        if (!dto.numeroIdentificacion.EsRucValido()) throw new ExcepcionOperativa(PersonasEventos.ERROR_RUC_INVALIDO);
                    }

                    result = await _personasRepository.ActualizarPersona(dto);

                    if (result == 0)
                    {
                        throw new ExcepcionOperativa(PersonasEventos.ACTUALIZAR_PERSONA_NO_ACTUALIZADO);
                    }

                    await _personasRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest {
                        codigoPersona = dto.codigoPersona,
                        fechaUsuarioActualiza = dto.fechaUsuarioRegistra ?? DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra
                    });

                    scope.Complete();

                    _logger.Informativo($"Persona actualizada");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ActualizarPersona=> {exo.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarPersona=> {ex}");
                        codigoEvento = PersonasEventos.ACTUALIZAR_PERSONA_ERROR;
                    }
                }

                _logger.Informativo($"ActualizarPersona => {codigoEvento}");

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

        public async Task<Respuesta> ObtenerPersona(long codigoPersona)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                Persona persona = null;
                int codigoEvento = PersonasEventos.OBTENER_PERSONA_OK;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo($"Obteniendo persona");

                    persona = await _personasRepository.ObtenerPersona(codigoPersona);
                    scope.Complete();

                    _logger.Informativo($"Persona obtenida");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ObtenerPersona=> {exo.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerPersona=> {ex}");
                        codigoEvento = PersonasEventos.OBTENER_PERSONA_ERROR;
                    }
                }

                _logger.Informativo($"ObtenerPersona => {codigoEvento}");

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                    _config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta()
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = persona
                };
            }
        }

        public async Task<Respuesta> ObtenerPersonaJoinMinimo(UltActPersonaRequest dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                int codigoEvento = PersonasEventos.OBTENER_PERSONA_OK;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
                Persona.PersonaJoinMinimo persona = null;

                try
                {
                    persona = await _personasRepository.ObtenerPersonaJoinMinimo(dto);
                    _logger.Informativo($"ObtenerPersonaJoinMinimo => {codigoEvento}");
                }
                catch (Exception ex)
                {
                    if (ex is ExcepcionOperativa operativa)
                    {
                        _logger.Error($"ObtenerPersonaJoinMinimo => {ex.InnerException}");
                        codigoEvento = operativa.codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerPersonaJoinMinimo => {ex}");
                        codigoEvento = PersonasEventos.OBTENER_PERSONAS_ERROR;
                    }

                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                }

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                    _config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta()
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = persona
                };
            }
        }

        public async Task<Respuesta> ObtenerPersonas(PersonaRequest dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                int codigoEvento = PersonasEventos.OBTENER_PERSONA_OK; // Se obtuvieron los correos
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
                List<PersonaResponse> personas = new List<PersonaResponse>();

                try
                {
                    personas = _personasRepository.ObtenerPersonas(dto);
                    _logger.Informativo($"ObtenerPersonas => {codigoEvento}");
                }
                catch (Exception ex)
                {
                    if (ex is ExcepcionOperativa)
                    {
                        _logger.Error($"ObtenerPersonas => {ex.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerPersonas => {ex}");
                        codigoEvento = PersonasEventos.OBTENER_PERSONAS_ERROR;
                    }

                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                }

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                    _config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta()
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = personas
                };
            }
        }
    }
}
