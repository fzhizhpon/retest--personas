using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.BienesInmuebles;
using Personas.Core.Entities.BienesInmuebles;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using VimaCoop.Excepciones;


namespace Personas.Application.Services
{
    public class BienesInmueblesService : IBienesInmueblesService
    {
        private readonly IBienesInmueblesRepository
            _bienesInmueblesRepository; // interfaz del repositorio correspondiente

        private readonly ConfiguracionApp _config; // configuracion
        protected readonly IMensajesRespuestaRepository _textoInfoService;
        protected readonly ILogsRepository<BienesInmuebles> _logger;

        public BienesInmueblesService(
            IBienesInmueblesRepository bienesInmueblesRepository,
            ConfiguracionApp config,
            ILogsRepository<BienesInmuebles> logger,
            IMensajesRespuestaRepository textoInfoService
        )
        {
            _bienesInmueblesRepository = bienesInmueblesRepository;
            _config = config;
            _textoInfoService = textoInfoService;
            _logger = logger;
        }

        public async Task<Respuesta> ObtenerBienesInmuebles(ObtenerBienesInmueblesDto dto)
        {
            // * variables para la respuesta de la query
            int codigo = 0;
            IEnumerable<BienesInmuebles.Minimo> bienesInmuebles = null;

            // * codigos de eventos
            int codigoEvento = BienesInmueblesEventos.OBTENER_BIENES_INMUEBLES;
            // * codigo interno de funcionamiento
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo("Obteniendo BienesInmuebles");
                    (codigo, bienesInmuebles) = await _bienesInmueblesRepository.ObtenerBienesInmuebles(dto);
                    _logger.Informativo("Obtenido BienesInmuebles");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ObtenerBienesInmuebles => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerBienesInmuebles=> {ex}");
                        codigoEvento = BienesInmueblesEventos.OBTENER_BIENES_INMUEBLES_ERROR;
                    }
                }
            }

            string textoInfo = await _textoInfoService.ObtenerTextoInfo(_config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = textoInfo,
                resultado = bienesInmuebles
            };
        }

        public async Task<Respuesta> ObtenerBienInmueble(long codigoPersona, long numeroRegistro)
        {

            // * variables para la respuesta de la query
            int codigo = 0;
            BienesInmuebles bienInmueble = null;

            // * codigos de eventos
            int codigoEvento = BienesInmueblesEventos.OBTENER_BIEN_INMUEBLE;
            // * codigo interno de funcionamiento
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo("Obteniendo BienInmueble");
                    (codigo, bienInmueble) =
                        await _bienesInmueblesRepository.ObtenerBienInmueble(codigoPersona, numeroRegistro);
                    _logger.Informativo("Obtenido BienInmueble");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ObtenerBienInmueble => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerBienInmueble=> {ex}");
                        codigoEvento = BienesInmueblesEventos.OBTENER_BIEN_INMUEBLE_ERROR;
                    }
                }
            }

            string textoInfo = await _textoInfoService.ObtenerTextoInfo(_config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = textoInfo,
                resultado = bienInmueble
            };
        }

        public async Task<Respuesta> GuardarBienesInmuebles(GuardarBienesInmueblesDto obj)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                // * numero de registro
                long numeroRegistro = 0;
                // * codigos de eventos
                int codigoEvento = BienesInmueblesEventos.GUARDAR_BIENES_INMUEBLES;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo("Obteniendo codigoNumeroRegistro...");
                    // recuperamos el ultimo id ingresado
                    numeroRegistro = await _bienesInmueblesRepository.ObtenerNumeroRegistroMax();
                    numeroRegistro += 1;
                    _logger.Informativo("Obtenido codigoNumeroRegistro...");

                    _logger.Informativo("Guardando BienesInmuebles...");
                    // * insercion
                    await _bienesInmueblesRepository.GuardarBienesInmuebles(
                        new GuardarBienesInmueblesDto()
                        {
                            codigoPersona = obj.codigoPersona,
                            numeroRegistro = numeroRegistro,
                            tipoBienInmueble = obj.tipoBienInmueble,
                            codigoPais = obj.codigoPais,
                            codigoProvincia = obj.codigoProvincia,
                            codigoCiudad = obj.codigoCiudad,
                            codigoParroquia = obj.codigoParroquia,
                            sector = obj.sector,
                            callePrincipal = obj.callePrincipal,
                            calleSecundaria = obj.calleSecundaria,
                            numero = obj.numero,
                            codigoPostal = obj.codigoPostal,
                            tipoSector = obj.tipoSector,
                            esMarginal = obj.esMarginal,
                            longitud = obj.longitud,
                            latitud = obj.latitud,
                            avaluoComercial = obj.avaluoComercial,
                            avaluoCatastral = obj.avaluoCatastral,
                            areaTerreno = obj.areaTerreno,
                            areaConstruccion = obj.areaConstruccion,
                            valorTerrenoMetrosCuadrados = obj.valorTerrenoMetrosCuadrados,
                            fechaConstruccion = obj.fechaConstruccion,
                            referencia = obj.referencia,
                            comunidad = obj.comunidad,
                            descripcion = obj.descripcion,
                            codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                            fechaUsuarioActualiza = DateTime.Now,
                            estado = '1'
                        }
                    );

                    _logger.Informativo("Guardado BienesInmuebles...");
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"GuardarBienesInmuebles => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"GuardarBienesInmuebles=> {ex}");
                        codigoEvento = BienesInmueblesEventos.GUARDAR_BIENES_INMUEBLES_ERROR;
                    }
                }

                string textoInfo =
                    await _textoInfoService.ObtenerTextoInfo(_config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta()
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = null
                };
            }
        }

        public async Task<Respuesta> ActualizarBienesInmuebles(ActualizarBienesInmueblesDto obj)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                // * codigos de eventos
                int codigoEvento = BienesInmueblesEventos.ACTUALIZAR_BIENES_INMUEBLES;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo("Actualizando BienesInmuebles");
                    int resultado = await _bienesInmueblesRepository.ActualizarBienesInmuebles(
                        new ActualizarBienesInmueblesDto()
                        {
                            codigoPersona = obj.codigoPersona,
                            numeroRegistro = obj.numeroRegistro,
                            tipoBienInmueble = obj.tipoBienInmueble,
                            //codigoPais = obj.codigoPais,
                            //codigoProvincia = obj.codigoProvincia,
                            //codigoCiudad = obj.codigoCiudad,
                            //codigoParroquia = obj.codigoParroquia,
                            //sector = obj.sector,
                            callePrincipal = obj.callePrincipal,
                            calleSecundaria = obj.calleSecundaria,
                            //numero = obj.numero,
                            //codigoPostal = obj.codigoPostal,
                            //tipoSector = obj.tipoSector,
                            //esMarginal = obj.esMarginal,
                            //longitud = obj.longitud,
                            //latitud = obj.latitud,
                            avaluoComercial = obj.avaluoComercial,
                            avaluoCatastral = obj.avaluoCatastral,
                            //areaTerreno = obj.areaTerreno,
                            areaConstruccion = obj.areaConstruccion,
                            valorTerrenoMetrosCuadrados = obj.valorTerrenoMetrosCuadrados,
                            fechaConstruccion = obj.fechaConstruccion,
                            referencia = obj.referencia,
                            comunidad = obj.comunidad,
                            descripcion = obj.descripcion,
                        });
                    _logger.Informativo("Actualizado BienesInmuebles");
                    if (resultado == 0)
                    {
                        throw new ExcepcionOperativa(BienesInmueblesEventos.BIENES_INMUEBLES_NO_ACTUALIZADO);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ActualizarBienesInmuebles => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ActualizarBienesInmuebles=> {ex}");
                        codigoEvento = BienesInmueblesEventos.ACTUALIZAR_BIENES_INMUEBLES_ERROR;
                    }
                }

                string textoInfo =
                    await _textoInfoService.ObtenerTextoInfo(_config.Idioma, codigoEvento, _config.Modulo);

                return new Respuesta()
                {
                    codigo = codigoRespuesta,
                    mensaje = textoInfo,
                    resultado = null
                };
            }
        }

        public async Task<Respuesta> EliminarBienesInmuebles(EliminarBienesInmueblesDto obj)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                // * codigos de eventos
                int codigoEvento = BienesInmueblesEventos.ELIMINAR_BIENES_INMUEBLES;
                int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

                try
                {
                    _logger.Informativo("Eliminando BienesInmuebles");
                    int resultado = await _bienesInmueblesRepository.EliminarBienesInmuebles(
                        new EliminarBienesInmueblesDto()
                        {
                            codigoPersona = obj.codigoPersona,
                            numeroRegistro = obj.numeroRegistro
                        });
                    _logger.Informativo("Eliminado BienesInmuebles");
                    if (resultado == 0)
                    {
                        throw new ExcepcionOperativa(BienesInmueblesEventos.BIENES_INMUEBLES_NO_ELIMINADO);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"EliminarBienesInmuebles => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"EliminarBienesInmuebles=> {ex}");
                        codigoEvento = BienesInmueblesEventos.ELIMINAR_BIENES_INMUEBLES_ERROR;
                    }
                }

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

        public async Task<Respuesta> ObtenerBienesInmueblesSinJoin(long codigoPersona)
        {
            // * variables para la respuesta de la query
            int codigo = 0;
            IEnumerable< BienesInmuebles.MinimoSinJoin> bienInmueble = null;

            // * codigos de eventos
            int codigoEvento = BienesInmueblesEventos.OBTENER_BIEN_INMUEBLE;
            // * codigo interno de funcionamiento
            int codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    _logger.Informativo("Obteniendo BienInmueble");
                    (codigo, bienInmueble) =
                        await _bienesInmueblesRepository.ObtenerBienesInmueblesSinJoin(codigoPersona);
                    _logger.Informativo("Obtenido BienInmueble");
                }
                catch (Exception ex)
                {
                    codigoRespuesta = CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO;

                    if (ex is ExcepcionOperativa)
                    {
                        ExcepcionOperativa exo = (ExcepcionOperativa)ex;
                        _logger.Error($"ObtenerBienInmueble => {exo}");
                        codigoEvento = ((ExcepcionOperativa)ex).codigoEvento;
                    }
                    else
                    {
                        _logger.Error($"ObtenerBienInmueble=> {ex}");
                        codigoEvento = BienesInmueblesEventos.OBTENER_BIEN_INMUEBLE_ERROR;
                    }
                }
            }

            string textoInfo = await _textoInfoService.ObtenerTextoInfo(_config.Idioma, codigoEvento, _config.Modulo);

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                mensaje = textoInfo,
                resultado = bienInmueble
            };
        }
    }
}