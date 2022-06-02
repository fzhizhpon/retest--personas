using System;
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
    public class PersonaNaturalService : IPersonaNaturalService
    {
        protected readonly IPersonaRepository _personaRepository;
        protected readonly IPersonaNaturalRepository _personaNaturalRepository;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<PersonaNaturalService> _logger;

        public PersonaNaturalService(IPersonaNaturalRepository repository,
            IPersonaRepository personaRepository,
            IMensajesRespuestaRepository textoInfoService,
            ILogsRepository<PersonaNaturalService> logger,
            ConfiguracionApp config)
        {
            _personaNaturalRepository = repository;
            _personaRepository = personaRepository;
            _textoInfoService = textoInfoService;
            _config = config;
            _logger = logger;
        }

        public async Task<Respuesta> GuardarPersonaNatural(GuardarPersonaNaturalDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                int result = 0;
                int codigoEvento = PersonasNaturalesEventos.GUARDAR_PERSONA_NATURAL_OK;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
                long codPersona = 0;

                try
                {
                    int nroPersonas = await _personaRepository.ObtenerPersonasPorIdentificacion(dto.numeroIdentificacion);

                    if (nroPersonas != 0)
                    {
                        throw new ExcepcionOperativa(PersonasEventos.ERROR_PERSONA_YA_EXISTE);
                    }

                    _logger.Informativo($"Obtener codigo persona...");

                    codPersona = await _personaRepository.ObtenerCodigoPersonaMax();
                    codPersona += 1 ;

                    _logger.Informativo($"Codigo persona obtenido");

                    if (dto.codigoTipoIdentificacion == 1) // Cedula
                    {
                        if (!dto.numeroIdentificacion.EsCedulaValida()) throw new ExcepcionOperativa(PersonasEventos.ERROR_CEDULA_INVALIDA);
                    }

                    _logger.Informativo($"Guardando persona...");

                    result = await _personaRepository.GuardarPersona(new
                    {
                        codigoPersona = codPersona,
                        numeroIdentificacion = dto.numeroIdentificacion,
                        fechaRegistro = DateTime.Now,
                        observaciones = dto.observaciones,
                        codigoTipoIdentificacion = dto.codigoTipoIdentificacion,
                        codigoTipoPersona = dto.codigoTipoPersona,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                        fechaUsuarioActualiza = DateTime.Now,
                        fechaUsuarioRegistra = DateTime.Now,
                        codigoTipoContribuyente = dto.codigoTipoContribuyente,
                        codigoAgencia = dto.codigoAgencia,
                        codigoUsuarioRegistra = _config.codigoUsuarioRegistra
                    });

                    _logger.Informativo($"Persona Guardada");

                    _logger.Informativo($"Guardando persona natural");

                    result = await _personaNaturalRepository.GuardarPersonaNatural( new
                    {
                        codigoPersona = codPersona,
                        nombres = dto.nombres,
                        apellidoPaterno = dto.apellidoPaterno,
                        apellidoMaterno = dto.apellidoMaterno,
                        fechaNacimiento = dto.fechaNacimiento,
                        tieneDiscapacidad = dto.tieneDiscapacidad,
                        codigoTipoDiscapacidad = dto.codigoTipoDiscapacidad,
                        porcentajeDiscapacidad = dto.porcentajeDiscapacidad,
                        codigoPaisNacimiento = dto.codigoPaisNacimiento,
                        codigoProvinciaNacimiento = dto.codigoProvinciaNacimiento,
                        codigoCiudadNacimiento = dto.codigoCiudadNacimiento,
                        codigoParroquiaNacimiento = dto.codigoParroquiaNacimiento,
                        codigoTipoSangre = dto.codigoTipoSangre,
                        codigoConyuge = dto.codigoConyuge,
                        codigoEstadoCivil = dto.codigoEstadoCivil,
                        codigoGenero = dto.codigoGenero,
                        codigoProfesion = dto.codigoProfesion,
                        codigoTipoEtnia = dto.codigoTipoEtnia,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                        fechaUsuarioActualiza = DateTime.Now,
                        vulnerable = dto.esVulnerable,
                    });

                    // Colocacion de conyugue
                    if (dto.codigoEstadoCivil == 1 || dto.codigoEstadoCivil == 5)
                    {
                        if (dto.codigoConyuge != null)
                        {
                            try
                            {
                                await _personaNaturalRepository.ActualizarConyugue(new ActualizarConyugueRequest
                                {
                                    codigoEstadoCivil = dto.codigoEstadoCivil,
                                    codigoConyuge = (long) dto.codigoConyuge,
                                    codigoPersona = codPersona
                                });
                            } catch (Exception ex)
                            {
                                _logger.Error($"ErrorActualizarConyugue=> {ex.InnerException}");
                            }
                        }
                    }

                    scope.Complete();

                    _logger.Informativo($"Persona natural guardada");                    

                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"GuardarPersonaNatural=> {exo.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarPersonaNatural=> {ex}");
                        codigoEvento = PersonasNaturalesEventos.GUARDAR_PERSONA_NATURAL_ERROR;
                    }
                }

                _logger.Informativo($"GuardarPersonaNatural => {codigoEvento}");

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                    _config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta()
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = codPersona
                };
            }
        }

        public async Task<Respuesta> ActualizarPersonaNatural(ActualizarPersonaNaturalDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                int result = 0;
                int codigoEvento = PersonasNaturalesEventos.ACTUALIZAR_PERSONA_NATURAL_OK;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo($"Actualizando persona natural...");

                    dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;
                    dto.fechaUsuarioActualiza = DateTime.Now;

                    result = await _personaNaturalRepository.ActualizarPersonaNatural(dto);

                    if (result == 0)
                    {
                        throw new ExcepcionOperativa(PersonasNaturalesEventos.ACTUALIZAR_PERSONA_NATURAL_NO_ACTUALIZADO);
                    }

                    // Colocacion de conyugue
                    if (dto.codigoEstadoCivil == 1 || dto.codigoEstadoCivil == 5)
                    {
                        if (dto.codigoConyuge != null)
                        {
                            try
                            {
                                await _personaNaturalRepository.ActualizarConyugue(new ActualizarConyugueRequest
                                {
                                    codigoEstadoCivil = dto.codigoEstadoCivil,
                                    codigoConyuge = (long)dto.codigoConyuge,
                                    codigoPersona = dto.codigoPersona
                                });
                            }
                            catch (Exception ex)
                            {
                                _logger.Error($"ErrorActualizarConyugue=> {ex.InnerException}");
                            }
                        }
                    }

                    scope.Complete();

                    _logger.Informativo($"Persona natural actualizada");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ActualizarPersonaNatural=> {exo.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarPersonaNatural=> {ex}");
                        codigoEvento = PersonasNaturalesEventos.ACTUALIZAR_PERSONA_NATURAL_ERROR;
                    }
                }

                _logger.Informativo($"ActualizarPersonaNatural => {codigoEvento}");

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

        public async Task<Respuesta> ObtenerInfoPesona(long codigoPersona)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                InfoPersonaNaturalDto persona = null;
                int codigoEvento = PersonasNaturalesEventos.OBTENER_INFO_PERSONA_NATURAL_OK;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo($"Obteniendo información persona natural...");

                    persona = await _personaNaturalRepository.ObtenerInfoPersona(codigoPersona);
                    scope.Complete();

                    _logger.Informativo($"Información persona natural obtenida");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ObtenerInfoPesona=> {exo.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerInfoPesona=> {ex}");
                        codigoEvento = PersonasNaturalesEventos.OBTENER_INFO_PERSONA_NATURAL_ERROR;
                    }
                }

                _logger.Informativo($"ObtenerInfoPesona => {codigoEvento}");

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

        public async Task<Respuesta> ObtenerPersonaNatural(long codigoPersona)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                PersonaNatural persona = null;
                int codigoEvento = PersonasNaturalesEventos.OBTENER_PERSONA_NATURAL_OK;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo($"Obteniendo persona natural...");

                    persona = await _personaNaturalRepository.ObtenerPersonaNatural(codigoPersona);
                    scope.Complete();

                    _logger.Informativo($"Persona natural obtenida");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ObtenerPersonaNatural=> {exo.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerPersonaNatural=> {ex}");
                        codigoEvento = PersonasNaturalesEventos.OBTENER_PERSONA_NATURAL_ERROR;
                    }                    
                }

                _logger.Informativo($"ObtenerPersonaNatural => {codigoEvento}");

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
    }
}
