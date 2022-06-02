using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos;
using Personas.Core.Dtos.Trabajos;
using Personas.Core.Entities.Trabajos;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using Personas.Infrastructure.Querys.Trabajos;
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
    public class TrabajosRepository : ITrabajosRepository
    {
        private readonly string _esquema;
        protected readonly ConfiguracionApp _config;
        protected readonly IDbConnection _conexionDb;
        private readonly MongoDriver.MongoClient _mongoClient;
        protected readonly IHistoricosRepository<Trabajo> _historicosRepository;
        protected readonly ILogsRepository<ReferenciasComercialesRepository> _logger;

        public TrabajosRepository(
            ConfiguracionApp config,
            IDbConnection conexionDb,
            IConfiguration configuration,
            IOptions<MongoOpciones> mongoConfig,
            IHistoricosRepository<Trabajo> historicosRepository,
            ILogsRepository<ReferenciasComercialesRepository> logger
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

        public async Task<int> GuardarTrabajo(int codigoTrabajo, GuardarTrabajoDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string consulta = TrabajosQueries.guardarTrabajo(_esquema);
                    int result = await _conexionDb.ExecuteAsync(consulta, new
                    {
                        codigoTrabajo = codigoTrabajo,
                        codigoPersona = dto.codigoPersona,
                        codigoCategoria = dto.codigoCategoria,
                        codigoPais = dto.codigoPais,
                        codigoProvincia = dto.codigoProvincia,
                        codigoCiudad = dto.codigoCiudad,
                        codigoParroquia = dto.codigoParroquia,
                        ingresosMensuales = dto.ingresosMensuales,
                        codigoTipoTiempo = dto.codigoTipoTiempo,
                        tiempoActividad = dto.tiempoActividad,
                        fechaIngreso = dto.fechaIngreso,
                        razonSocial = dto.razonSocial,
                        direccion = dto.direccion,
                        cargo = dto.cargo,
                        principal = dto.principal,
                        codigoActividad = dto.codigoActividad,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                        fechaUsuarioActualiza = DateTime.Now
                    });

                    scope.Complete();

                    return result;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(TrabajosEventos.GUARDAR_TRABAJO_ERROR, exc);
                }
            }
        }

        public async Task<int> ObtenerCodigoTrabajo(int codigoPersona)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    string consulta = TrabajosQueries.obtenerNuevoCodigo(_esquema);
                    var result = await _conexionDb.QueryAsync<int>(consulta, new { codigoPersona = codigoPersona });

                    int codigo = 1;

                    if (result.FirstOrDefault() != 0) codigo = result.First();

                    scope.Complete();

                    return codigo;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(TrabajosEventos.OBTENER_TRABAJO_ERROR, exc);
                }
            }
        }

        public async Task<Trabajo> ObtenerTrabajo(ObtenerTrabajoDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string consulta = TrabajosQueries.obtenerTrabajo(_esquema);
                    Trabajo trabajo = await _conexionDb.QueryFirstOrDefaultAsync<Trabajo>(consulta, dto);

                    scope.Complete();
                    return trabajo;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(TrabajosEventos.OBTENER_TRABAJO_ERROR, exc);
                }
            }
        }

        public async Task<IList<Trabajo.TrabajoMinimo>> ObtenerTrabajos(ObtenerTrabajosDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string consulta = TrabajosQueries.obtenerTrabajos(_esquema);

                    var trabajos = await _conexionDb
                        .QueryAsync<Trabajo.TrabajoMinimo>(consulta, new
                        {
                            codigoPersona = dto.codigoPersona,
                            indiceInicial = dto.indiceInicial,
                            numeroRegistros = dto.numeroRegistros
                        });

                    scope.Complete();
                    return trabajos.ToList();
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(TrabajosEventos.OBTENER_VARIOS_TRABAJOS_ERROR, exc);
                }
            }
        }

        public async Task<int> EliminarTrabajo(EliminarTrabajoDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string query = TrabajosQueries.eliminarTrabajo(_esquema);
                    int result = await _conexionDb.ExecuteAsync(query, dto);

                    scope.Complete();
                    return result;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(TrabajosEventos.ELIMINAR_TRABAJO_ERROR, exc);
                }
            }
        }

        public async Task<int> ActualizarTrabajo(ActualizarTrabajoDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string query = TrabajosQueries.actualizarTrabajo(_esquema);
                    int result = await _conexionDb.ExecuteAsync(query, new
                    {
                        codigoTrabajo = dto.codigoTrabajo,
                        codigoPersona = dto.codigoPersona,
                        codigoCategoria = dto.codigoCategoria,
                        codigoPais = dto.codigoPais,
                        codigoProvincia = dto.codigoProvincia,
                        codigoCiudad = dto.codigoCiudad,
                        codigoParroquia = dto.codigoParroquia,
                        ingresosMensuales = dto.ingresosMensuales,
                        codigoTipoTiempo = dto.codigoTipoTiempo,
                        tiempoActividad = dto.tiempoActividad,
                        fechaIngreso = dto.fechaIngreso,
                        razonSocial = dto.razonSocial,
                        direccion = dto.direccion,
                        cargo = dto.cargo,
                        principal = dto.principal,
                        codigoActividad = dto.codigoActividad,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                        fechaUsuarioActualiza = DateTime.Now
                    });

                    

                    scope.Complete();
                    return result;
                }
                catch (Exception exc)
                {
                    Console.WriteLine("ERROR============================");
                    Console.WriteLine(exc);
                    Console.WriteLine("ERROR============================");
                    throw new ExcepcionOperativa(TrabajosEventos.ACTUALIZAR_TRABAJO_ERROR, exc);
                }
            }
        }

        public async Task GuardarTrabajoHistorico(Trabajo trabajo)
        {

            using (var session = await _mongoClient.StartSessionAsync())
            {
                try
                {
                    MongoDriver.IMongoDatabase db = _mongoClient.GetDatabase($"Historico_{_config.Modulo}");
                    MongoDriver.IMongoCollection<Trabajo> coll = db.GetCollection<Trabajo>(TrabajosQueries.obtenerTabla());

                    //var transactionOptions = new MongoDriver.TransactionOptions(
                    //    readPreference: MongoDriver.ReadPreference.Primary,
                    //    readConcern: MongoDriver.ReadConcern.Local,
                    //    writeConcern: MongoDriver.WriteConcern.WMajority);

                    //var cancellationToken = CancellationToken.None; // normally a real token would be used
                    //var result = session.WithTransaction(
                    //    (s, ct) =>
                    //    {
                    //        coll.InsertOneAsync(s, trabajo, cancellationToken: ct);
                    //        return "Inserted into collections in different databases";
                    //    },
                    //    transactionOptions,
                    //    cancellationToken);

                    session.StartTransaction();
                    await coll.InsertOneAsync(trabajo);
                    await session.CommitTransactionAsync();
                }
                catch (Exception exc)
                {
                    await session.AbortTransactionAsync();
                    throw new ExcepcionOperativa(TrabajosEventos.ACTUALIZAR_TRABAJO_ERROR, exc);
                }
            }
        }

        //private async Task<bool> guardarTrabajoHistorico(Trabajo trabajo)
        //{
        //    trabajo.guid = _config.guid;

        //    HistoricoDto<Trabajo> historicoDto = new HistoricoDto<Trabajo>();

        //    historicoDto.guid = _config.guid;
        //    historicoDto.modulo = _config.Modulo;
        //    historicoDto.tabla = TrabajosQueries.obtenerTabla();
        //    historicoDto.referencia = trabajo;

        //    _logger.Informativo($"Guardando el trabajo en histórico...");

        //    bool resultHistorico = await _historicosRepository.GuardarHistorico(historicoDto);
        //    if (!resultHistorico)
        //    {
        //        _logger.Error($"El trabajo no se pudo guardar en histórico");
        //        return false;
        //    }

        //    _logger.Informativo($"Trabajo guardado en histórico");
        //    return true;
        //}

    }
}
