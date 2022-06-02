using Catalogo.Core.DTOs;
using Catalogo.Core.Interfaces.DataBase;
using Catalogo.Core.Interfaces.IRepositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalogo.Infrastructure.Repositories
{
    public class AgenciaRepository : IAgenciaRepository
    {
        private readonly IConexion<OracleConnection> _conexion;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly string _esquema;

        public AgenciaRepository(IConexion<OracleConnection> conexion, ILogger<AgenciaRepository> logger,
            IConfiguration configuration)
        {
            _conexion = conexion;
            _logger = logger;
            _configuration = configuration;
            _esquema = _configuration["EsquemaDb"];
        }

        public async Task<(int, IEnumerable<ComboDto>)> SelectAgenciasPorSucursal(ObtenerAgenciaSucursalDto dto)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                try
                {
                    connection.Open();

                    string query = Queries.AgenciaQuery.SelectAgenciasPorSucursal(_esquema);

                    var result = await connection.QueryAsync<ComboDto>(query, dto);

                    return (CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO, result);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ERROR: Error en el metodo SelectAgenciasPorSucursal, {ex}");
                    return (CodigosLogicaInterna.CODIGO_GENERICO_ERROR_INTERNO, null);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
