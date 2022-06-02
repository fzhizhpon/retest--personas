using Dapper;
using Microsoft.Extensions.Configuration;
using Personas.Application.CodigosEventos;
using Personas.Core.Dtos.Accionistas;
using Personas.Core.Interfaces.IRepositories;
using Personas.Infrastructure.Querys.Accionistas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using VimaCoop.Excepciones;
using Vimasistem.QueryFilter.Interfaces;

namespace Personas.Infrastructure.Repositories
{
    public class AccionistaRepository : IAccionistaRepository
    {
        private readonly string _esquema;
        private IDbConnection _connection;
        private readonly IPagination _pagination;

        public AccionistaRepository(IConfiguration configuration, IDbConnection connection, IPagination pagination)
        {
            _esquema = configuration["EsquemaDb"];
            _connection = connection;
            _pagination = pagination;
        }

        public async Task<int> GuardarAccionistas(List<GuardarAccionistaDto> accionistasDto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string query = AccionistaQuery.GuardarAccionistas(_esquema);
                    int result = await _connection.ExecuteAsync(query, accionistasDto);

                    scope.Complete();
                    return result;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionOperativa(AccionistasEventos.GUARDAR_LISTA_ACCIONISTAS_ERROR, ex);
                }
            }
        }

        public async Task<int> ActualizarAccionista(ActualizarAccionistaDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string query = AccionistaQuery.ActualizarAccionista(_esquema);
                    int result = await _connection.ExecuteAsync(query, dto);

                    scope.Complete();
                    return result;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionOperativa(AccionistasEventos.ACTUALIZAR_ACCIONISTA_ERROR, ex);
                }
            }
        }

        public async Task<IEnumerable<AccionistaResponse>> ObtenerAccionistas(AccionistaRequest dto)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    string query = AccionistaQuery.ObtenerAccionistas(_esquema);
                    string[] tables = new[] { "PERS_ACCIONISTAS:a" };
                    string filter = dto.GetQuery(tables);

                    if (!string.IsNullOrEmpty(filter))
                    {
                        query = query + " WHERE " + filter;
                    }

                    query += " " + _pagination.GetQuery(dto.indiceInicial, dto.numeroRegistros);

                    IEnumerable<AccionistaResponse> result = await _connection.QueryAsync<AccionistaResponse>(query, dto);

                    scope.Complete();

                    return result;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionOperativa(AccionistasEventos.OBTENER_ACCIONISTA_ERROR, ex);
                }
            }
        }
    }
}
