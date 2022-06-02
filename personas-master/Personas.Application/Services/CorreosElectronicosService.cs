using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.CorreosElectronicos;
using Personas.Core.Dtos.Personas;
using Personas.Core.Entities.CorreosElectronicos;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{
    public class CorreosElectronicosService : ICorreosElectronicosService
	{
        protected readonly ICorreosElectronicosRepository _correosRepository;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<DireccionesService> _logger;
        private readonly IPersonaRepository _personaRepository;

        public CorreosElectronicosService(ICorreosElectronicosRepository repository,
            IMensajesRespuestaRepository textoInfoService,
            ILogsRepository<DireccionesService> logger,
            ConfiguracionApp config,
            IPersonaRepository personaRepository
        )
        {
            _correosRepository = repository;
            _textoInfoService = textoInfoService;
            _config = config;
            _logger = logger;
            _personaRepository = personaRepository;
        }

        public async Task<Respuesta> ObtenerCorreos(int codigoPersona)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                int codigoEvento = CorreosElectronicosEventos.LEIDO_OK; // Se obtuvieron los correos
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
                List<CorreoElectronicoDto> correos = new List<CorreoElectronicoDto>();

                try
                {
                    correos = _correosRepository.ObtenerCorreos(codigoPersona);
                    scope.Complete();
                    _logger.Informativo($"ObtenerCorreos => {codigoEvento}");
                }
                catch (Exception ex)
                {
                    if (ex is ExcepcionOperativa)
                    {
                        _logger.Error($"ObtenerCorreos => {ex.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerCorreos => {ex}");
                        codigoEvento = CorreosElectronicosEventos.ERROR_OBTENER_CORREOS;
                    }

                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                }

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                    _config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta()
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = correos
                };
            }
        }

        public async Task<Respuesta> AgregarCorreo(AgregarCorreoElectronicoDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                int codigoEvento = CorreosElectronicosEventos.GUARDADO_OK; // Se guardo el correo
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
                int codigoCorreo = -1;

                try
                {
                    CorreoElectronico correoBuscado = _correosRepository.ObtenerCorreo(dto.codigoPersona, dto.correoElectronico);

                    if (correoBuscado is not null)
                    {
                        throw new ExcepcionOperativa(CorreosElectronicosEventos.ERROR_CORREO_EXISTE);
                    }

                    if (dto.esPrincipal == '1')
                    {
                        _correosRepository.DesmarcarCorreoPrincipal(dto.codigoPersona);
                    }
                    else
                    {
                        if (_correosRepository.NroCorreosPrincipales(dto.codigoPersona) == 0)
                        {
                            dto.esPrincipal = '1';
                        }
                    }

                    codigoCorreo = _correosRepository.ObtenerCodigoNuevoCorreo(dto.codigoPersona);

                    _correosRepository.AgregarCorreo(codigoCorreo, dto);

                    await _personaRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest
                    {
                        codigoPersona = dto.codigoPersona,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra
                    });

                    _logger.Informativo($"AgregarCorreo => {codigoEvento}");

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    if (ex is ExcepcionOperativa)
                    {
                        _logger.Error($"AgregarCorreo => {ex.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"AgregarCorreo => {ex}");
                        codigoEvento = CorreosElectronicosEventos.ERROR_OBTENER_CORREOS;
                    }

                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                }

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = codigoCorreo
                };
            }
        }

        public async Task<Respuesta> ActualizarCorreo(ActualizarCorreoElectronicoDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                int codigoEvento = CorreosElectronicosEventos.ACTUALIZADO_OK; // Se actualizo el correo
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    CorreoElectronico correo;

                    // Validamos que el correo a actualizar no exista
                    #region
                    correo = _correosRepository.ObtenerCorreo(dto.codigoPersona, dto.correoElectronico);

                    if (correo != null)
                    {
                        if (correo.codigoCorreoElectronico != dto.codigoCorreoElectronico)
                        {
                            throw new ExcepcionOperativa(CorreosElectronicosEventos.ERROR_CORREO_EXISTE);
                        }
                    }
                    #endregion

                    correo = _correosRepository.ObtenerCorreo(dto.codigoPersona, dto.codigoCorreoElectronico);

                    if (correo is null)
                    {
                        throw new ExcepcionOperativa(CorreosElectronicosEventos.ERROR_OBTENER_CORREO);
                    }

                    correo.correoElectronico = dto.correoElectronico;
                    correo.esPrincipal = dto.esPrincipal;
                    correo.observaciones = dto.observaciones;
                    correo.fechaActualiza = DateTime.Now;
                    correo.codigoUsuarioActualiza = _config.codigoUsuarioRegistra;

                    if (correo.esPrincipal == '1')
                    {
                        _correosRepository.DesmarcarCorreoPrincipal(dto.codigoPersona);
                    }
                    else
                    {
                        if (_correosRepository.NroCorreosPrincipales(dto.codigoPersona) == 0)
                        {
                            correo.esPrincipal = '1';
                        }
                    }

                    int filasAfectadas = _correosRepository.ActualizarCorreo(correo);

                    await _personaRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest
                    {
                        codigoPersona = dto.codigoPersona,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra
                    });

                    _logger.Informativo($"ActualizarCorreo => {codigoEvento}");
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        _logger.Error($"ActualizarCorreo => {((ExcepcionOperativa)ex).codigoEvento} {ex.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarCorreo => {ex}");
                        codigoEvento = CorreosElectronicosEventos.ERROR_ACTUALIZAR_CORREO;
                    }
                }

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                    _config.Idioma, codigoEvento, _config.Modulo);

                scope.Dispose();

                return new Respuesta
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = null
                };
            }
        }

        public async Task<Respuesta> EliminarCorreo(EliminarCorreoRequest dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                int codigoEvento = CorreosElectronicosEventos.ELIMINADO_OK; // Se guardo el correo
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _correosRepository.EliminarCorreo(dto);

                    _logger.Informativo($"EliminarCorreo => {codigoEvento}");

                    await _personaRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest
                    {
                        codigoPersona = dto.codigoPersona,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra
                    });

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    if (ex is ExcepcionOperativa)
                    {
                        _logger.Error($"EliminarCorreo => {ex.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerCorreos => {ex}");
                        codigoEvento = CorreosElectronicosEventos.ERROR_ELIMINAR_CORREO;
                    }

                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                }

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = null
                };
            }
        }
    }
}

