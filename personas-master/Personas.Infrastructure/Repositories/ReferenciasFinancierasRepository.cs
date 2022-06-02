using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos;
using Personas.Core.Dtos.ReferenciasFinancieras;
using Personas.Core.Entities.ReferenciasFinancieras;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using Personas.Infrastructure.Querys.ReferenciasFinancieras;
using VimaCoop.Excepciones;
using MongoDriver = MongoDB.Driver;

namespace Personas.Infrastructure.Repositories
{
    public class ReferenciasFinancierasRepository : IReferenciasFinancierasRepository
    {
        private readonly string _esquema;
        IOptions<MongoOpciones> _mongoConfig;
        private readonly ConfiguracionApp _config;
        protected readonly IDbConnection _conexionDb;
        private readonly MongoDriver.MongoClient _mongoClient;
        protected readonly ILogsRepository<ReferenciasFinancierasRepository> _logger;
        protected readonly IHistoricosRepository<ReferenciaFinanciera> _historicosRepository;

        public ReferenciasFinancierasRepository(
            ConfiguracionApp config,
            IDbConnection conexionDb,
            IConfiguration configuration,
            IOptions<MongoOpciones> mongoConfig,
            ILogsRepository<ReferenciasFinancierasRepository> logger,
            IHistoricosRepository<ReferenciaFinanciera> historicosRepository
        )
        {
            _config = config;
            _logger = logger;
            _conexionDb = conexionDb;
            _esquema = configuration["EsquemaDb"];
            _historicosRepository = historicosRepository;
            MongoDriver.MongoClientSettings clientSettings =
                MongoDriver.MongoClientSettings.FromConnectionString(mongoConfig.Value.Connection);
            //// Timeout de mongo, configurado en Nacos por defecto a 3000 MS
            //clientSettings.ServerSelectionTimeout = TimeSpan.FromMilliseconds(3000);
            _mongoClient = new MongoDriver.MongoClient(clientSettings);
        }

        public async Task<int> GuardarReferenciaFinanciera(GuardarReferenciaFinancieraDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    string consulta = ReferenciasFinancierasQueries.guardarReferenciaFinanciera(_esquema);
                    int result = await _conexionDb.ExecuteAsync(consulta, dto);

                    scope.Complete();
                    return result;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(ReferenciasFinancierasEventos.GUARDAR_REFERENCIA_FINANCIERA_ERROR, exc);
                }
            }
        }

        public async Task<int> ObtenerCodigoReferenciaFinanciera(long codigoPersona)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    string consulta = ReferenciasFinancierasQueries.obtenerNuevoCodigo(_esquema);
                    int result = await _conexionDb.QueryFirstAsync<int>(consulta, new { codigoPersona = codigoPersona });
                    result++;

                    scope.Complete();

                    return result;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(ReferenciasComercialesEventos.GUARDAR_REFERENCIA_COMERCIAL_ERROR, exc);
                }
            }
        }

        public async Task<ReferenciaFinanciera>
            ObtenerReferenciaFinanciera(ObtenerReferenciaFinancieraDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    string consulta = ReferenciasFinancierasQueries.obtenerReferenciaFinanciera(_esquema);

                    ReferenciaFinanciera refFinanciera =
                        await _conexionDb.QueryFirstOrDefaultAsync<ReferenciaFinanciera>(consulta, dto);

                    scope.Complete();
                    return refFinanciera;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(ReferenciasFinancierasEventos.OBTENER_REFERENCIA_FINANCIERA_ERROR, exc);
                }
            }
        }

        public async Task<IList<ReferenciaFinanciera>>
            ObtenerReferenciasFinancieras(ObtenerReferenciasFinancierasDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    string consulta = ReferenciasFinancierasQueries.obtenerReferenciasFinancieras(_esquema);

                    var refPersonales = await _conexionDb
                        .QueryAsync<ReferenciaFinanciera>(consulta, new
                        {
                            codigoPersona = dto.codigoPersona,
                            indiceInicial = dto.paginacion.indiceInicial,
                            numeroRegistros = dto.paginacion.numeroRegistros
                        });

                    scope.Complete();
                    return refPersonales.ToList();
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(ReferenciasFinancierasEventos.OBTENER_VARIAS_REFERENCIAS_FINANCIERAS_ERROR, exc);
                }
            }
        }

        public async Task<int> EliminarReferenciaFinanciera(EliminarReferenciaFinancieraDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    string consulta = ReferenciasFinancierasQueries.eliminarReferenciaFinanciera(_esquema);
                    int result = await _conexionDb.ExecuteAsync(consulta, dto);

                    scope.Complete();
                    return result;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(ReferenciasFinancierasEventos.ELIMINAR_REFERENCIA_FINANCIERA_ERROR, exc);
                }
            }
        }

        public async Task<int> ActualizarReferenciaFinanciera(ActualizarReferenciaFinancieraDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    string consulta = ReferenciasFinancierasQueries.actualizarReferenciaFinanciera(_esquema);
                    int result = await _conexionDb.ExecuteAsync(consulta, dto);

                    scope.Complete();
                    return result;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(ReferenciasFinancierasEventos.ELIMINAR_REFERENCIA_FINANCIERA_ERROR, exc);
                }
            }
        }

        public async Task GuardarReferenciaFinancieraHistorico(ReferenciaFinanciera refFinanciera)
        {

            using (var session = await _mongoClient.StartSessionAsync())
            {
                try
                {
                    MongoDriver.IMongoDatabase db = _mongoClient.GetDatabase($"Historico_{_config.Modulo}");
                    MongoDriver.IMongoCollection<ReferenciaFinanciera> coll =
                        db.GetCollection<ReferenciaFinanciera>(ReferenciasFinancierasQueries.obtenerTabla());

                    session.StartTransaction();
                    await coll.InsertOneAsync(refFinanciera);
                    await session.CommitTransactionAsync();
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(TrabajosEventos.ACTUALIZAR_TRABAJO_ERROR, exc);
                }
            }
        }

        //using (_connection = _iConexion.ObtenerConexion())
        //{
        //    _connection.Open();

        //    using (_transaction = _connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
        //    using (var sessionMongoDb = await _historicosRepository.ObtenerClienteMongo().StartSessionAsync())
        //    {
        //        try
        //        {
        //            // Obtengo la referencia financiera
        //            ReferenciaFinanciera referenciaFinanciera = await obtenerReferenciaFinanciera(
        //                dto.codigoPersona.GetValueOrDefault(),
        //                dto.codigoReferenciaFinanciera.GetValueOrDefault());
        //            if (referenciaFinanciera == null)
        //            {
        //                return (CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO, -1);
        //            }

        //            _logger.Informativo($"Actualizando referencia financiera...");
        //            // Actualizo la referencia financiera
        //            string query = ReferenciasFinancierasQueries.actualizarReferenciaFinanciera(_esquema);
        //            int result = await _connection.ExecuteAsync(query, dto);

        //            // Guardo la referencia financiera en historico
        //            sessionMongoDb.StartTransaction();
        //            bool seGuardoHistorico = await guardarReferenciaFinancieraHistorico(referenciaFinanciera);
        //            if (!seGuardoHistorico)
        //            {
        //                return (CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO, -1);
        //            }

        //            // Commit en transaccion oracle
        //            _transaction.Commit();
        //            // Commit en transaccion mongo
        //            await sessionMongoDb.CommitTransactionAsync();

        //            _logger.Informativo($"Referencia financiera actualizada");
        //            return (CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO, result);
        //        }
        //        catch (Exception exc)
        //        {
        //            _transaction.Rollback();
        //            await sessionMongoDb.AbortTransactionAsync();
        //            _logger.Error($"ActualizarReferenciaFinanciera Try Catch => {exc.ToString()}");
        //            return (CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO, -1);
        //        }
        //        finally
        //        {
        //            sessionMongoDb.Dispose();
        //            _connection.Close();
        //        }
        //    }
        //}


        //private async Task<ReferenciaFinanciera> obtenerReferenciaFinanciera(int codigoPersona, int codigoReferenciaFinanciera)
        //{
        //    string consulta = ReferenciasFinancierasQueries.obtenerReferenciaFinanciera(_esquema);

        //    _logger.Informativo($"Consultando referencia financiera...");
        //    ReferenciaFinanciera referenciaFinanciera = await
        //        _connection.QueryFirstOrDefaultAsync<ReferenciaFinanciera>(consulta, new
        //        {
        //            codigoPersona = codigoPersona,
        //            codigoReferenciaFinanciera = codigoReferenciaFinanciera
        //        });

        //    if (referenciaFinanciera == null)
        //    {
        //        _logger.Error($"No se encontró referencia financiera");
        //    }
        //    else
        //    {
        //        _logger.Informativo($"Referencia financiera encontrada");
        //    }
        //    return referenciaFinanciera;
        //}

        //private async Task<bool> guardarReferenciaFinancieraHistorico(ReferenciaFinanciera referenciaFinanciera)
        //{
        //    referenciaFinanciera.guid = _config.guid;

        //    HistoricoDto<ReferenciaFinanciera> historicoDto = new HistoricoDto<ReferenciaFinanciera>();

        //    historicoDto.guid = _config.guid;
        //    historicoDto.modulo = _config.Modulo;
        //    historicoDto.tabla = ReferenciasFinancierasQueries.obtenerTabla();
        //    historicoDto.referencia = referenciaFinanciera;

        //    _logger.Informativo($"Guardando la referencia financiera en histórico...");

        //    bool resultHistorico = await _historicosRepository.GuardarHistorico(historicoDto);
        //    if (!resultHistorico)
        //    {
        //        _logger.Error($"La referencia financiera no se pudo guardar en histórico");
        //        return false;
        //    }

        //    _logger.Informativo($"Referencia financiera guardada en histórico");
        //    return true;
        //}


    }
}
