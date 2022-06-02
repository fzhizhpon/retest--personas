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
    public class PersonaNoNaturalService : IPersonaNoNaturalService
    {
        protected readonly IPersonaRepository _personaRepository;
        protected readonly IPersonaNoNaturalRepository _personaNoNaturalRepository;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<PersonaNoNaturalService> _logger;

        public PersonaNoNaturalService(IPersonaNoNaturalRepository repository,
            IPersonaRepository personaRepository,
            IMensajesRespuestaRepository textoInfoService,
            ILogsRepository<PersonaNoNaturalService> logger,
            ConfiguracionApp config)
        {
            _personaNoNaturalRepository = repository;
            _personaRepository = personaRepository;
            _textoInfoService = textoInfoService;
            _config = config;
            _logger = logger;
        }

        public async Task<Respuesta> GuardarPersonaNoNatural(GuardarPersonaNoNaturalDto dto)
          {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                int result = 0;
                int codigoEvento = PersonasNoNaturalesEventos.GUARDAR_PERSONA_NO_NATURAL_OK;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
                long codPersona = 0;

                try
                {
                    int nroPersonas = await _personaRepository.ObtenerPersonasPorIdentificacion(dto.numeroIdentificacion);

                    if (nroPersonas != 0)
                    {
                        throw new ExcepcionOperativa(PersonasEventos.ERROR_PERSONA_YA_EXISTE);
                    }

                    dto.codigoUsuarioRegistra = _config.codigoUsuarioRegistra;
                    dto.fechaUsuarioRegistra = DateTime.Now;

                    _logger.Informativo($"Obtener codigo persona...");

                    //if (dto.codigoTipoIdentificacion == 2) // RUC
                    //{
                    //    if (!dto.numeroIdentificacion.EsRucValido()) throw new ExcepcionOperativa(PersonasEventos.ERROR_RUC_INVALIDO);
                    //}

                    codPersona = await _personaRepository.ObtenerCodigoPersonaMax();
                    codPersona += 1;

                    _logger.Informativo($"Codigo persona obtenido");

                    _logger.Informativo($"Guardando persona...");

                    result = await _personaRepository.GuardarPersona(new
                    {
                        codigoPersona = codPersona,
                        numeroIdentificacion = dto.numeroIdentificacion,
                        fechaRegistro = DateTime.Now,
                        observaciones = dto.observaciones,
                        codigoTipoIdentificacion = dto.codigoTipoIdentificacion,
                        codigoTipoPersona = dto.codigoTipoPersona,
                        codigoUsuarioRegistra = _config.codigoUsuarioRegistra,
                        fechaUsuarioRegistra = DateTime.Now,
                        codigoTipoContribuyente = dto.codigoTipoContribuyente,
                        codigoAgencia = dto.codigoAgencia
                    });

                    _logger.Informativo($"Persona Guardada");

                    _logger.Informativo($"Guardando persona no natural");

                    result = await _personaNoNaturalRepository.GuardarPersonaNoNatural(new
                    {
                        codigoPersona = codPersona,
                        razonSocial = dto.razonSocial,
                        fechaConstitucion = dto.fechaConstitucion,
                        objetoSocial = dto.objetoSocial,
                        tipoSociedad = dto.tipoSociedad,
                        finalidadLucro = dto.finalidadLucro,
                        obligadoLlevarContabilidad = dto.obligadoLlevarContabilidad,
                        agenteRetencion = dto.agenteRetencion,
                        direccionWeb = dto.direccionWeb,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                        fechaUsuarioActualiza = DateTime.Now
                    });

                    scope.Complete();

                    _logger.Informativo($"Persona no natural guardada");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"GuardarPersonaNoNatural=> {exo.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarPersonaNoNatural=> {ex}");
                        codigoEvento = PersonasNoNaturalesEventos.GUARDAR_PERSONA_NO_NATURAL_ERROR;
                    }
                }

                _logger.Informativo($"GuardarPersonaNoNatural => {codigoEvento}");

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

        public async Task<Respuesta> ObtenerPersonaNoNatural(long codigoPersona)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                PersonaNoNatural persona = null;
                int codigoEvento = PersonasNoNaturalesEventos.OBTENER_PERSONA_NO_NATURAL_OK;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo($"Obteniendo persona no natural...");

                    persona = await _personaNoNaturalRepository.ObtenerPersonaNoNatural(codigoPersona);
                    scope.Complete();

                    _logger.Informativo($"Persona no natural obtenida");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ObtenerPersonaNoNatural=> {exo.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerPersonaNoNatural=> {ex}");
                        codigoEvento = PersonasNoNaturalesEventos.OBTENER_PERSONA_NO_NATURAL_ERROR;
                    }
                }

                _logger.Informativo($"ObtenerPersonaNoNatural => {codigoEvento}");

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

        public async Task<Respuesta> ActualizarPersonaNoNatural(ActualizarPersonaNoNaturalDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                int result = 0;
                int codigoEvento = PersonasNoNaturalesEventos.ACTUALIZAR_PERSONA_NO_NATURAL_OK;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo($"Actualizando persona no natural...");

                    dto.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;
                    dto.fechaUsuarioActualiza = DateTime.Now;

                    result = await _personaNoNaturalRepository.ActualizarPersonaNoNatural(dto);

                    if (result == 0)
                    {
                        throw new ExcepcionOperativa(PersonasNoNaturalesEventos.ACTUALIZAR_PERSONA_NO_NATURAL_NO_ACTUALIZADO);
                    }

                    scope.Complete();

                    _logger.Informativo($"Persona no natural actualizada");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ActualizarPersonaNoNatural=> {exo.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarPersonaNoNatural=> {ex}");
                        codigoEvento = PersonasNoNaturalesEventos.ACTUALIZAR_PERSONA_NO_NATURAL_ERROR;
                    }
                }

                _logger.Informativo($"ActualizarPersonaNoNatural => {codigoEvento}");

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
    }
}
