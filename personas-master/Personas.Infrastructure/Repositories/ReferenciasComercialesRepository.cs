using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Microsoft.Extensions.Configuration;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.ReferenciasComerciales;
using Personas.Core.Entities.ReferenciasComerciales;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using Personas.Infrastructure.Querys.ReferenciasComerciales;
using VimaCoop.Excepciones;

namespace Personas.Infrastructure.Repositories
{
    public class ReferenciasComercialesRepository : IReferenciasComercialesRepository
    {

        private readonly string _esquema;
        private readonly ConfiguracionApp _config;
        protected readonly IDbConnection _conexionDb;
        protected readonly ILogsRepository<ReferenciasComercialesRepository> _logger;
        protected readonly IHistoricosRepository<ReferenciaComercial> _historicosRepository;

        public ReferenciasComercialesRepository(
            ConfiguracionApp config,
            IDbConnection conexionDb,
            IConfiguration configuration,
            ILogsRepository<ReferenciasComercialesRepository> logger,
            IHistoricosRepository<ReferenciaComercial> historicosRepository)
        {
            _logger = logger;
            _config = config;
            _conexionDb = conexionDb;
            _esquema = configuration["EsquemaDb"];
            _historicosRepository = historicosRepository;
        }

        public async Task<int> GuardarReferenciaComercial(GuardarReferenciaComercialDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    string consulta = ReferenciasComercialesQueries.guardarReferenciaComercial(_esquema);
                    int result = await _conexionDb.ExecuteAsync(consulta, dto);

                    scope.Complete();
                    return result;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(ReferenciasComercialesEventos.GUARDAR_REFERENCIA_COMERCIAL_ERROR, exc);
                }
            }
        }

        public async Task<int> ObtenerCodigoReferenciaComercial(int codigoPersona)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    string consulta = ReferenciasComercialesQueries.obtenerNuevoCodigo(_esquema);
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

        public async Task<ReferenciaComercial> ObtenerReferenciaComercial(ObtenerReferenciaComercialDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    string consulta = ReferenciasComercialesQueries.obtenerReferenciaComercial(_esquema);
                    ReferenciaComercial refComercial = await _conexionDb.QueryFirstOrDefaultAsync<ReferenciaComercial>(consulta, dto);

                    scope.Complete();
                    return refComercial;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(ReferenciasComercialesEventos.OBTENER_REFERENCIA_COMERCIAL_ERROR, exc);
                }
            }
        }

        public async Task<IList<ReferenciaComercial.ReferenciaComercialMinimo>>
            ObtenerReferenciasComerciales(ObtenerReferenciasComercialesDto dto)
        {
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    string consulta = ReferenciasComercialesQueries.obtenerReferenciasComerciales(_esquema);

                    var refComerciales = await _conexionDb
                        .QueryAsync<ReferenciaComercial.ReferenciaComercialMinimo>(consulta, new
                        {
                            codigoPersona = dto.codigoPersona,
                            indiceInicial = dto.paginacion.indiceInicial,
                            numeroRegistros = dto.paginacion.numeroRegistros
                        });

                    scope.Complete();
                    return refComerciales.ToList();
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(ReferenciasComercialesEventos.OBTENER_VARIAS_REFERENCIAS_COMERCIALES_ERROR, exc);
                }
            }
        }

        public async Task<int> ActualizarReferenciaComercial(ActualizarReferenciaComercialDto dto)
        {

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    string query = ReferenciasComercialesQueries.actualizarReferenciaComercial(_esquema);
                    int result = await _conexionDb.ExecuteAsync(query, dto);

                    scope.Complete();
                    return result;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(ReferenciasComercialesEventos.ACTUALIZAR_REFERENCIA_COMERCIAL_ERROR, exc);
                }
            }
        }

        public async Task<int> EliminarReferenciaComercial(EliminarReferenciaComercialDto dto)
        {

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            ))
            {
                try
                {
                    string query = ReferenciasComercialesQueries.eliminarReferenciaComercial(_esquema);
                    int result = await _conexionDb.ExecuteAsync(query, dto);

                    scope.Complete();
                    return result;
                }
                catch (Exception exc)
                {
                    throw new ExcepcionOperativa(ReferenciasComercialesEventos.ELIMINAR_REFERENCIA_COMERCIAL_ERROR, exc);
                }
            }
        }

        //private async Task<bool> guardarReferenciaComercialHistorico(ReferenciaComercial referenciaComercial)
        //{
        //    referenciaComercial.guid = _config.guid;

        //    HistoricoDto<ReferenciaComercial> historicoDto = new HistoricoDto<ReferenciaComercial>();

        //    historicoDto.guid = _config.guid;
        //    historicoDto.modulo = _config.Modulo;
        //    historicoDto.tabla = ReferenciasComercialesQueries.obtenerTabla();
        //    historicoDto.referencia = referenciaComercial;

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
