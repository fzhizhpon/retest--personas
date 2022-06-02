using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos;
using Personas.Core.Dtos.Representantes;
using Personas.Core.Entities.Representantes;
using Personas.Core.Entities.Trabajos;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using Personas.Infrastructure.Querys.Representante;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using VimaCoop.Excepciones;
using MongoDriver = MongoDB.Driver;

namespace Personas.Infrastructure.Repositories
{
    public class RepresentantesRepository : IRepresentantesRepository
    {

        private readonly string _esquema;
        protected readonly ConfiguracionApp _config;
        protected readonly IDbConnection _conexionDb;
        private readonly MongoDriver.MongoClient _mongoClient;
        protected readonly ILogsRepository<RepresentantesRepository> _logger;
        protected readonly IHistoricosRepository<Representante> _historicosRepository;

        public RepresentantesRepository(
            ConfiguracionApp config,
            IDbConnection conexionDb,
            IConfiguration configuration,
            IOptions<MongoOpciones> mongoConfig,
            ILogsRepository<RepresentantesRepository> logger,
            IHistoricosRepository<Representante> historicosRepository
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

        public async Task<int> GuardarRepresentante(GuardarRepresentanteDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string consulta = RepresentantesQueries.guardarRepresentante(_esquema);
                    int result = await _conexionDb.ExecuteAsync(consulta, dto);
                    scope.Complete();

                    return result;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(RepresentantesEventos.GUARDAR_REPRESENTANTE_ERROR, exc);
                }
            }
        }

        public async Task<int> ActualizarRepresentante(ActualizarRepresentanteDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string consulta = RepresentantesQueries.actualizarRepresentante(_esquema);
                    int result = await _conexionDb.ExecuteAsync(consulta, dto);
                    scope.Complete();

                    return result;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(RepresentantesEventos.GUARDAR_REPRESENTANTE_ERROR, exc);
                }
            }
        }

        public async Task<int> EliminarRepresentante(EliminarRepresentanteDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string consulta = RepresentantesQueries.eliminarRepresentante(_esquema);
                    int result = await _conexionDb.ExecuteAsync(consulta, dto);
                    scope.Complete();

                    return result;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(RepresentantesEventos.ELIMINAR_REPRESENTANTE_ERROR, exc);
                }
            }
        }

        public Task GuardarRepresentanteHistorico(Representante Representante)
        {
            throw new NotImplementedException();
        }

        public async Task<Representante.RepresentanteJoin> ObtenerRepresentante(ObtenerRepresentanteDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string consulta = RepresentantesQueries.obtenerRepresentanteJoin(_esquema);
                    Representante.RepresentanteJoin result = await _conexionDb.QueryFirstOrDefaultAsync<Representante.RepresentanteJoin>(consulta, dto);
                    scope.Complete();

                    return result;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(RepresentantesEventos.GUARDAR_REPRESENTANTE_ERROR, exc);
                }
            }
        }

        public async Task<IList<Representante.RepresentanteJoin>> ObtenerRepresentantes(ObtenerRepresentantesDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string consulta = RepresentantesQueries.obtenerRepresentantesJoin(_esquema);
                    var result = await _conexionDb.QueryAsync<Representante.RepresentanteJoin>(consulta, dto);
                    scope.Complete();

                    return result.ToList();
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(RepresentantesEventos.GUARDAR_REPRESENTANTE_ERROR, exc);
                }
            }
        }

        public async Task<IList<Representante.RepresentanteSimple>> ObtenerRepresentantesPrincipales(ObtenerRepresentantesDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string consulta = RepresentantesQueries.obtenerRepresentantesPrincipales(_esquema);
                    var representantes = await _conexionDb.QueryAsync<Representante.RepresentanteSimple>(consulta, dto);
                    scope.Complete();

                    return representantes.ToList();
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(RepresentantesEventos.GUARDAR_REPRESENTANTE_ERROR, exc);
                }
            }
        }

        public async Task<int> ActualizarRepresentantesPrincipales(IList<Representante.RepresentanteSimple> dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string consulta = RepresentantesQueries.actualizarRepresentantePrincipal(_esquema);
                    var representantes = await _conexionDb.ExecuteAsync(consulta, dto);
                    scope.Complete();

                    return representantes;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(RepresentantesEventos.ACTUALIZAR_REPRESENTANTE_PRINCIPAL_ERROR, exc);
                }
            }
        }

        public async Task<Representante.RepresentanteSimple> ObtenerRepresentanteMinimo(ObtenerRepresentanteDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string consulta = RepresentantesQueries.obtenerRepresentanteMinimo(_esquema);
                    Representante.RepresentanteSimple representante = 
                        await _conexionDb.QueryFirstOrDefaultAsync<Representante.RepresentanteSimple>(consulta, dto);
                    scope.Complete();

                    return representante;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(RepresentantesEventos.GUARDAR_REPRESENTANTE_ERROR, exc);
                }
            }
        }
    }
}
