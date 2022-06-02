using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using Dapper;
using Microsoft.Extensions.Configuration;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.CorreosElectronicos;
using Personas.Core.Entities.CorreosElectronicos;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using Personas.Infrastructure.Querys.CorreosElectronicos;
using VimaCoop.Excepciones;

namespace Personas.Infrastructure.Repositories
{
    public class CorreosElectronicosRepository : ICorreosElectronicosRepository
    {
        private IDbConnection _conexion;
        private readonly string _esquema;
        private readonly ConfiguracionApp _config;
        private readonly ILogsRepository<CorreosElectronicosRepository> _logger;

        public CorreosElectronicosRepository(ILogsRepository<CorreosElectronicosRepository> logger,
            IConfiguration configuration, ConfiguracionApp config, IDbConnection conexion)
        {
            _logger = logger;
            _esquema = configuration["EsquemaDb"];
            _config = config;
            _conexion = conexion;
        }

        public List<CorreoElectronicoDto> ObtenerCorreos(int codigoPersona)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string query = CorreosQueries.ObtenerTodos(_esquema);

                    List<CorreoElectronicoDto> correos = _conexion.Query<CorreoElectronicoDto>(query, new
                    {
                        codigoPersona = codigoPersona
                    }).ToList();

                    scope.Complete();
                    return correos;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionOperativa(CorreosElectronicosEventos.ERROR_OBTENER_CORREOS, ex);
                }
            }
        }

        public int DesmarcarCorreoPrincipal(int codigoPersona)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string query = CorreosQueries.QuitarPrincipal(_esquema);
                    int filasAfectadas = _conexion.Execute(query, new { codigoPersona = codigoPersona });

                    scope.Complete();
                    return filasAfectadas;
                } catch (Exception ex)
                {
                    throw new ExcepcionOperativa(CorreosElectronicosEventos.ERROR_DESMARCAR_PRINCIPAL, ex);
                }
            }
        }

        public int NroCorreosPrincipales(int codigoPersona)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string query = CorreosQueries.ContarCorreosPrincipales(_esquema);
                    int registros = _conexion.QueryFirst<int>(query, new { codigoPersona = codigoPersona });

                    scope.Complete();
                    return registros;
                } catch (Exception ex)
                {
                    throw new ExcepcionOperativa(CorreosElectronicosEventos.ERROR_CONTAR_PRINCIPALES, ex);
                }
            }
        }

        public int ObtenerCodigoNuevoCorreo(int codigoPersona)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    string query = CorreosQueries.ObtenerNuevoCodigo(_esquema);
                    int codigoCorreo = _conexion.QueryFirst<int>(query, new { codigoPersona = codigoPersona });
                    codigoCorreo += 1;

                    scope.Complete();
                    return codigoCorreo;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionOperativa(CorreosElectronicosEventos.ERROR_GENERAR_CODIGO, ex);
                }
            }
        }

        public int AgregarCorreo(int codigoCorreo, AgregarCorreoElectronicoDto dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string query = CorreosQueries.AgregarCorreo(_esquema);
                    int filasAfectadas = _conexion.Execute(query, new
                    {
                        codigoCorreoElectronico = codigoCorreo,
                        codigoPersona = dto.codigoPersona,
                        correoElectronico = dto.correoElectronico,
                        esPrincipal = dto.esPrincipal,
                        observaciones = dto.observaciones,
                        codigoUsuarioActualiza = _config.codigoUsuarioRegistra,
                        fechaActualiza = DateTime.Now,
                    });

                    scope.Complete();

                    _logger.Informativo($"OK: AgregarCorreo => {codigoCorreo}");
                    return filasAfectadas;
                }
                catch (Exception ex)
                {
                    _logger.Error($"AgregarCorreo => {ex}");
                    throw new ExcepcionOperativa(CorreosElectronicosEventos.ERROR_INSERTAR_CORREO, ex);
                }
            }
        }

        public CorreoElectronico ObtenerCorreo(int codigoPersona, int codigoCorreoElectronico)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string query = CorreosQueries.ObtenerCorreoCodigo(_esquema);
                    IEnumerable<CorreoElectronico> correos = _conexion.Query<CorreoElectronico>(query, new
                    {
                        codigoPersona = codigoPersona,
                        codigoCorreoElectronico = codigoCorreoElectronico
                    });

                    scope.Complete();

                    if(correos.Count() == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return correos.First();
                    }
                }
                catch (Exception ex)
                {
                    throw new ExcepcionOperativa(CorreosElectronicosEventos.ERROR_OBTENER_CORREO, ex);
                }
            }
        }

        public CorreoElectronico ObtenerCorreo(int codigoPersona, string correoElectronico)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string query = CorreosQueries.ObtenerCorreoNombre(_esquema);
                    IEnumerable<CorreoElectronico> correos = _conexion.Query<CorreoElectronico>(query, new {
                        codigoPersona = codigoPersona,
                        correoElectronico = correoElectronico
                    });

                    scope.Complete();

                    if (correos.Count() == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return correos.First();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw new ExcepcionOperativa(CorreosElectronicosEventos.ERROR_OBTENER_CORREO, ex);
                }
            }
        }

        public int ActualizarCorreo(CorreoElectronico correo)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string query = CorreosQueries.ActualizarCorreo(_esquema);

                    int result = _conexion.Execute(query, new
                    {
                        codigoPersona = correo.codigoPersona,
                        codigoCorreoElectronico = correo.codigoCorreoElectronico,
                        correoElectronico = correo.correoElectronico,
                        esPrincipal = correo.esPrincipal,
                        observaciones = correo.observaciones,
                        codigoUsuarioActualiza = correo.codigoUsuarioActualiza,
                        fechaActualiza = correo.fechaActualiza,
                        estado = correo.estado
                    });

                    scope.Complete();
                    return result;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionOperativa(CorreosElectronicosEventos.ERROR_ACTUALIZAR_CORREO, ex);
                }
            }
        }

        public int EliminarCorreo(EliminarCorreoRequest dto)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string query = CorreosQueries.EliminarCorreo(_esquema);
                    int filasAfectadas = _conexion.Execute(query, dto);

                    scope.Complete();
                    return filasAfectadas;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionOperativa(CorreosElectronicosEventos.ERROR_ELIMINAR_CORREO, ex);
                }
            }
        }
    }
}

