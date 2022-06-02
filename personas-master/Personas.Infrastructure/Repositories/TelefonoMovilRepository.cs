using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Personas.Core.App;
using Personas.Core.Dtos.TelefonoMovil;
using Personas.Core.Entities.TelefonosMovil;
using Personas.Core.Interfaces.DataBase;
using Personas.Core.Interfaces.IRepositories;
using Personas.Infrastructure.Querys.TelefonoMovil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vimasistem.QueryFilter.Interfaces;

namespace Personas.Infrastructure.Repositories
{
    public class TelefonoMovilRepository : ITelefonoMovilRepository
    {
        private OracleTransaction _transaction;
        private OracleConnection _connection;
        protected readonly IConexion<OracleConnection> _iConexion;
        private readonly string _esquema;
        private readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<TelefonoMovilRepository> _logger;
        private readonly IPagination _pagination;

        public TelefonoMovilRepository(IConexion<OracleConnection> conexion,
            ILogsRepository<TelefonoMovilRepository> logger, IConfiguration configuration,
            ConfiguracionApp config, IPagination pagination)
        {
            _iConexion = conexion;
            _logger = logger;
            _esquema = configuration["EsquemaDb"];
            _config = config;
            _pagination = pagination;
        }

        public async Task<(int, int)> GuardarTelefonoMovil(GuardarTelefonoMovilDto dto)
        {
            using (_connection = _iConexion.ObtenerConexion())
            {
                _connection.Open();

                using (_transaction = _connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        string query;
                        
                        if (dto.principal == '1')
                        {
                            query = TelefonoMovilQuery.QuitarTelefonoPrincipal(_esquema);
                            await _connection.ExecuteAsync(query, new { codigoPersona = dto.codigoPersona });
                        }
                        else
                        {
                            query = TelefonoMovilQuery.ContarTelefonosPrincipales(_esquema);
                            var registros = await _connection.QueryAsync<int>(query, new { codigoPersona = dto.codigoPersona });

                            if (registros.FirstOrDefault() == 0)
                            {
                                dto.principal = '1';
                            }
                        }

                        query = TelefonoMovilQuery.obtenerNuevoCodigo(_esquema);
                        int codigo = (await _connection.QueryAsync<int>(query, new { codigoPersona = dto.codigoPersona })).FirstOrDefault();

                        codigo = codigo + 1;

                        query = TelefonoMovilQuery.GuardarTelefonoMovil(_esquema);
                        int result = await _connection.ExecuteAsync(query, new {
                            codigoTelefonoMovil = codigo,
                            codigoPersona = dto.codigoPersona,
                            codigoPais = dto.codigoPais,
                            numero = dto.numero,
                            codigoOperadora = dto.codigoOperadora,
                            observaciones = dto.observaciones,
                            principal = dto.principal,
                            codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                            fechaUsuarioActualiza = DateTime.Now
                        });

                        _transaction.Commit();
                        _logger.Informativo($"OK: GuardarTelefonoMovilDto");
                        return (CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO, result);
                    }
                    catch (Exception exc)
                    {
                        _transaction.Rollback();
                        _logger.Error($"GuardarTelefonoMovilDto=> {exc}");
                        return (CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO, -1);
                    }
                    finally
                    {
                        _connection.Close();
                    }
                }
            }
        }

        public async Task<(int, IEnumerable<TelefonoMovil>)> ObtenerTelefonosMovil(ObtenerTelefonosMovilDto dto)
        {
            using (var connection = _iConexion.ObtenerConexion())
            {
                try
                {
                    string consulta = TelefonoMovilQuery.ObtenerTelefonosMovil(_esquema);
                    consulta = consulta + _pagination.GetQuery(dto.indiceInicial, dto.numeroRegistros);

                    _logger.Informativo($"Consultando: ObtenerTelefonosMovil");

                    connection.Open();
                    var result = await connection.QueryAsync<TelefonoMovil>(consulta, dto);

                    _logger.Informativo($"OK ObtenerTelefonosMovil");

                    return (CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO, result);
                }
                catch (Exception exc)
                {
                    _logger.Error($"ObtenerTelefonosMovil => {exc}");
                    return (CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO, null);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<(int, int)> ActualizarTelefonoMovil(ActualizarTelefonoMovilDto dto)
        {
            using (_connection = _iConexion.ObtenerConexion())
            {
                _connection.Open();

                using (_transaction = _connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        string query = TelefonoMovilQuery.ActualizarTelefonoMovil(_esquema);

                        int result = await _connection.ExecuteAsync(query, new
                        {
                            codigoTelefonoMovil = dto.codigoTelefonoMovil,
                            codigoPersona = dto.codigoPersona,
                            codigoOperadora = dto.codigoOperadora,
                            observaciones = dto.observaciones,
                            principal = dto.principal,
                            codigoEstado = '1',
                            codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                            fechaUsuarioActualiza = DateTime.Now
                        });

                        _transaction.Commit();
                        _logger.Informativo($"OK: ActualizarTelefonoMovil => {result}");
                        return (CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO, result);
                    }
                    catch (Exception exc)
                    {
                        _transaction.Rollback();
                        _logger.Error($"ActualizarTelefonoMovil => {exc}");
                        return (CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO, -1);
                    }
                    finally
                    {
                        _connection.Close();
                    }
                }
            }
        }

        public async Task<(int, int)> EliminarTelefonoMovil(EliminarTelefonoMovilDto dto)
        {
            using (_connection = _iConexion.ObtenerConexion())
            {
                _connection.Open();

                using (_transaction = _connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        string query = TelefonoMovilQuery.EliminarTelefonoMovil(_esquema);

                        int result = await _connection.ExecuteAsync(query, new
                        {
                            codigoPersona = dto.codigoPersona,
                            codigoTelefonoMovil = dto.codigoTelefonoMovil,
                            fechaUsuarioActualiza = DateTime.Now,
                            codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                        });

                        _transaction.Commit();
                        _logger.Informativo($"OK: EliminarTelefonoMovil => {result}");
                        return (CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO, result);
                    }
                    catch (Exception exc)
                    {
                        _transaction.Rollback();
                        _logger.Error($"EliminarTelefonoMovil => {exc}");
                        return (CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO, -1);
                    }
                    finally
                    {
                        _connection.Close();
                    }
                }
            }
        }
    }
}
