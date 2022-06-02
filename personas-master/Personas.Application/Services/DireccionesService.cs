using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Application.EnumsEventos;
using Personas.Core.App;
using Personas.Core.Dtos.Direcciones;
using Personas.Core.Dtos.Personas;
using Personas.Core.Dtos.TelefonosFijos;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;

namespace Personas.Application.Services
{
    public class DireccionesService : IDireccionesService
    {
        private readonly IDireccionesRepository _direccionesRepository;
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<DireccionesService> _logger;
        private readonly IPersonaRepository _personaRepository;
        private readonly ITelefonosFijosRepository _telefonosFijosRepository;

        public DireccionesService(IDireccionesRepository repository,
            IMensajesRespuestaRepository textoInfoService,
            ILogsRepository<DireccionesService> logger,
            ConfiguracionApp config,
            IPersonaRepository personaRepository,
            ITelefonosFijosRepository telefonosFijosRepository
        )
        {
            _direccionesRepository = repository;
            _textoInfoService = textoInfoService;
            _config = config;
            _logger = logger;
            _personaRepository = personaRepository;
            _telefonosFijosRepository = telefonosFijosRepository;
        }

        public async Task<Respuesta> GuardarDireccion(GuardarDireccionDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                int codigoEvento = DireccionesEventos.GUARDADO_OK; // Se guardo el correo
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
                int numeroRegistroDireccion = 0;
                int numeroRegistroTelefono = 0;

                try
                {

                    if (dto.principal == '1')
                    {
                        await _direccionesRepository.DesmarcarDireccionPrincipal(dto.codigoPersona);
                    }
                    else
                    {
                        if ((await _direccionesRepository.NroDireccionesPrincipales(dto.codigoPersona)) == 0)
                        {
                            dto.principal = '1';
                        }
                    }

                    numeroRegistroDireccion = await _direccionesRepository.ObtenerCodigoNuevaDireccion(dto.codigoPersona);
                    numeroRegistroTelefono = await _telefonosFijosRepository.GenerarNumeroRegistro(dto.codigoPersona);

                    dto.numeroRegistro = numeroRegistroDireccion;
                    dto.codigoUsuarioRegistra = _config.codigoUsuarioRegistra;
                    dto.fechaUsuarioRegistra = DateTime.Now;


                    await _direccionesRepository.GuardarDireccion(dto);

                    await _telefonosFijosRepository.GuardarTelefonoFijo(new GuardarTelefonoFijoDto()
                    {
                        codigoPersona = dto.codigoPersona,
                        codigoDireccion = numeroRegistroDireccion,
                        numeroRegistro = numeroRegistroTelefono,
                        numero = dto.telefonoFijo.numero,
                        codigoOperadora = dto.telefonoFijo.codigoOperadora,
                        observaciones = dto.telefonoFijo.observaciones,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra
                    });

                    await _personaRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest
                    {
                        codigoPersona = dto.codigoPersona,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra
                    });

                    _logger.Informativo($"AgregarDireccion => {codigoEvento}");

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    if (ex is ExcepcionOperativa)
                    {
                        _logger.Error($"AgregarDireccion => {ex.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"AgregarDireccion => {ex}");
                        codigoEvento = DireccionesEventos.ERROR_GUARDAR_DIRECCION;
                    }

                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                }

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = numeroRegistroDireccion
                };
            }
        }

        public async Task<Respuesta> ActualizarDireccion(ActualizarDireccionDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                int codigoEvento = DireccionesEventos.ACTUALIZADO_OK; // Se guardo el correo
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    if (dto.principal == '1')
                    {
                        await _direccionesRepository.DesmarcarDireccionPrincipal(dto.codigoPersona);
                    }
                    else
                    {
                        if ((await _direccionesRepository.NroDireccionesPrincipales(dto.codigoPersona)) == 0)
                        {
                            dto.principal = '1';
                        }
                    }

                    int afectados = await _direccionesRepository.ActualizarDireccion(dto);

                    if (afectados == 0)
                    {
                        codigoEvento = DireccionesEventos.ERROR_ACTUALIZAR_DIRECCION; // Se guardo el correo
                        codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                        throw new ExcepcionOperativa(codigoEvento);
                    }

                    await _telefonosFijosRepository.ActualizarTelefonoFijo(new ActualizarTelefonoFijoDto()
                    {
                        codigoPersona = dto.codigoPersona,
                        codigoDireccion = dto.numeroRegistro,
                        numeroRegistro = dto.telefonoFijo.numeroRegistro,
                        numero = dto.telefonoFijo.numero,
                        codigoOperadora = dto.telefonoFijo.codigoOperadora,
                        observaciones = dto.telefonoFijo.observaciones,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra
                    });

                    await _personaRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest
                    {
                        codigoPersona = dto.codigoPersona,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra
                    });

                    _logger.Informativo($"ActualizarDireccion => {codigoEvento}");

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    if (ex is ExcepcionOperativa)
                    {
                        _logger.Error($"ActualizarDireccion => {ex.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarDireccion => {ex}");
                        codigoEvento = DireccionesEventos.ERROR_ACTUALIZAR_DIRECCION;
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

        public async Task<Respuesta> EliminarDireccion(EliminarDireccionDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                int codigoEvento = DireccionesEventos.ELIMINADO_OK; // Se guardo el correo
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    await _telefonosFijosRepository.EliminarTelefonoFijo(new EliminarTelefonoFijoDto()
                    {
                        codigoPersona = dto.codigoPersona,
                        numeroRegistro = dto.numeroRegistro,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra
                    });

                    await _direccionesRepository.EliminarDireccion(dto);

                    await _personaRepository.ColocarFechaUltimaActualizacion(new UltActPersonaRequest
                    {
                        codigoPersona = dto.codigoPersona,
                        fechaUsuarioActualiza = DateTime.Now,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra
                    });

                    _logger.Informativo($"EliminarDireccion => {codigoEvento}");

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    if (ex is ExcepcionOperativa)
                    {
                        _logger.Error($"EliminarDireccion => {ex.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"EliminarDireccion => {ex}");
                        codigoEvento = DireccionesEventos.ERROR_ELIMINAR_DIRECCION;
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

        public async Task<Respuesta> ObtenerDireccion(ObtenerDireccionDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                int codigoEvento = DireccionesEventos.LEIDO_OK; // Se guardo el correo
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
                DireccionOutDto direccion = new DireccionOutDto();

                try
                {
                    direccion = await _direccionesRepository.ObtenerDireccion(dto);

                    direccion.direccion.numeroRegistro = dto.numeroRegistro;

                    _logger.Informativo($"ObtenerDirecciones => {codigoEvento}");

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    if (ex is ExcepcionOperativa)
                    {
                        _logger.Error($"ObtenerDirecciones => {ex.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerDirecciones => {ex}");
                        codigoEvento = DireccionesEventos.ERROR_OBTENER_DIRECCION;
                    }

                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                }

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = direccion
                };
            }
        }

        public async Task<Respuesta> ObtenerDirecciones(ObtenerDireccionesDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                int codigoEvento = DireccionesEventos.LEIDOS_VARIOS_OK; // Se guardo el correo
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;
                List<DireccionMinResponse> direcciones = new List<DireccionMinResponse>();

                try
                {
                    direcciones = await _direccionesRepository.ObtenerDirecciones(dto);

                    _logger.Informativo($"ObtenerDirecciones => {codigoEvento}");

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    if (ex is ExcepcionOperativa)
                    {
                        _logger.Error($"ObtenerDirecciones => {ex.InnerException}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerDirecciones => {ex}");
                        codigoEvento = DireccionesEventos.ERROR_OBTENER_DIRECCIONES;
                    }

                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;
                }

                string textoInfo = await _textoInfoService.ObtenerTextoInfo(
                _config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = direcciones
                };
            }
        }
    }
}