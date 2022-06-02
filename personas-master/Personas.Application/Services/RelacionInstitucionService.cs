
using Microsoft.Extensions.Configuration;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.RelacionInstitucion;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{
    public class RelacionInstitucionService : IRelacionInstitucionService
    {
        protected readonly IRelacionInstitucionRepository _relacionInstitucionRepository;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<RelacionInstitucionService> _logger;

        public RelacionInstitucionService(IRelacionInstitucionRepository repository,
            IMensajesRespuestaRepository textoInfoService,
            ILogsRepository<RelacionInstitucionService> logger,
            ConfiguracionApp config)
        {
            _relacionInstitucionRepository = repository;
            _textoInfoService = textoInfoService;
            _config = config;
            _logger = logger;
        }

        public async Task<Respuesta> ActualizarPersonaRelacionInstitucion(PersonaRelacionInstitucion dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                int result = 0;
                int codigoEvento = RelacionInstitucionEventos.ACTUALIZAR_RELACION_INSTITUCION;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo($"Actualizando relacion institucion");

                    result = await _relacionInstitucionRepository.ActualizarPersonaRelacionInstitucion(dto);

                    if (result == 0)
                    {
                        throw new ExcepcionOperativa(PersonasEventos.ACTUALIZAR_PERSONA_NO_ACTUALIZADO);
                    }

                    scope.Complete();

                    _logger.Informativo($"Persona relacion institucion");
                }
                catch (System.Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ActualizarRelacionInstitucion => {exo.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarRelacionInstitucion => {ex}");
                        codigoEvento = PersonasEventos.ACTUALIZAR_PERSONA_ERROR;
                    }
                }

                _logger.Informativo($"ActualizarRelacionInstitucion => {codigoEvento}");

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

        public async Task<Respuesta> GuardarPersonaRelacionInstitucion(PersonaRelacionInstitucion dto)
        {

            using (TransactionScope scope = new TransactionScope(
                 TransactionScopeOption.Required,
                 new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted},
                 TransactionScopeAsyncFlowOption.Enabled
             ))
            
            {
                int codigoEvento = RelacionInstitucionEventos.GUARDAR_RELACION_INSTITUCION; 
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

               
                try
                {
                    dto.usuarioAsigna = _config.codigoUsuarioRegistra;
                    dto.codigoAgenciaRegistra = _config.codigoAgencia;
                    PersonaRelacionInstitucion personaRelacionInstitucion = await _relacionInstitucionRepository.ObtenerRelacionInstitucionMin(dto);
                    
                    int result;
                    if (personaRelacionInstitucion != null)
                    {
                        dto.estado = "1";
                        dto.fechaAsignacion = DateTime.Now;
                        result = await _relacionInstitucionRepository.ActualizarPersonaRelacionInstitucion(dto);

                    }
                    else {
                        result = await _relacionInstitucionRepository.GuardarPersonaRelacionInstitucion(dto);
                    }

                    if (result == 0)
                    {
                        codigoEvento = RelacionInstitucionEventos.RELACION_INSTITUCION_NO_GUARDADO;
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }


                    _logger.Informativo($"AgregarPersonaRelacionInstitucion ");

                    scope.Complete();
                }
                catch (System.Exception ex)
                {
                    if (ex is ExcepcionOperativa)
                    {
                        _logger.Error($"AgregarPersonaRelacionInstitucion => {ex.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"AgregarPersonaRelacionInstitucion => {ex}");
                        codigoEvento = RelacionInstitucionEventos.GUARDAR_RELACION_INSTITUCION_ERROR;
                    }

                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                }

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = null //numeroRegistroDireccion
                };
            }
        }

        public async Task<Respuesta> ObtenerRelacionInstitucion(RelacionInstitucion dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                int codigoEvento = PersonasEventos.OBTENER_PERSONA_OK; // Se obtuvieron los correos
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
                List<RelacionInstitucion> personarelacionInstitucion = new List<RelacionInstitucion>();
               
                try
                {
                    personarelacionInstitucion = await _relacionInstitucionRepository.ObtenerRelacionInstitucion(dto);
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
                    resultado = personarelacionInstitucion
                };
            }
        }

        public async Task<Respuesta> ObtenerRelacionInstitucionMin(PersonaRelacionInstitucion dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                int codigoEvento = PersonasEventos.OBTENER_PERSONA_OK; // Se obtuvieron los correos
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
                PersonaRelacionInstitucion personarelacionInstitucion = new PersonaRelacionInstitucion();

                try
                {
                    personarelacionInstitucion = await _relacionInstitucionRepository.ObtenerRelacionInstitucionMin(dto);
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
                    resultado = personarelacionInstitucion
                };
            }
        }
    }
}
