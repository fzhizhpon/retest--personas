using Consul;
using CoopCrea.Cross.Cache.Src;
using CoopCrea.Cross.Discovery.Consul;
using CoopCrea.Cross.Discovery.Fabio;
using CoopCrea.Cross.Discovery.Mvc;
using CoopCrea.Cross.Event.Src;
using CoopCrea.Cross.Http.Src;
using CoopCrea.Cross.Log.Src;
using CoopCrea.Cross.Tracing.Src;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oracle.ManagedDataAccess.Client;
using System.Reflection;
using Personas.Application.Messages.CommandHandlers;
using Personas.Application.Messages.Commands;
using Personas.Application.Middleware;
using Personas.Application.Services;
using Personas.Core.Interfaces.DataBase;
using Personas.Core.Interfaces.IRepositories;
using Personas.Core.Interfaces.IServices;
using Personas.Infrastructure.Configuraciones;
using Personas.Infrastructure.Repositories;
using Personas.Infrastructure.Validadores;
using Personas.Core.App;
using Vimasistem.QueryFilter.Interfaces;
using Vimasistem.QueryFilter.Implementations;
using Personas.Core.Dtos;
using System.Data;
using System.Threading;
using Personas.Application.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Text;

namespace Personas.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ThreadPool.SetMinThreads(50, 50);

            // Clase compartida para toda la vida de una request
            services.AddScoped<ConfiguracionApp>();
            /*Inicio - Inyeccion dependencias Servicios y Repositorios*/
            services.AddScoped<FiltroAuditoria>();

            services.AddHttpClient();

            services.Configure<MongoOpciones>(opt =>
            {
                opt.Connection = Configuration.GetSection("MongoDb:ConnectionString").Value;
                opt.DatabaseName = Configuration.GetSection("MongoDb:Database").Value;
            });

            services.AddTransient(typeof(IHistoricosRepository<>), typeof(HistoricosRepository<>));

            services.AddTransient(typeof(ILogsRepository<>), typeof(LogsRepository<>));

            services.AddTransient<ITelefonosFijosService, TelefonosFijosService>();
            services.AddTransient<ITelefonosFijosRepository, TelefonosFijosRepository>();

            services.AddTransient<IEstadosFinancierosService, EstadosFinancierosService>();
            services.AddTransient<IEstadosFinancierosRepository, EstadosFinancierosRepository>();

            services.AddTransient<IFamiliaresService, FamiliaresService>();
            services.AddTransient<IFamiliaresRepository, FamiliaresRepository>();

            services.AddTransient<IRepresentantesService, RepresentantesService>();
            services.AddTransient<IRepresentantesRepository, RepresentantesRepository>();

            services.AddTransient<ITrabajosService, TrabajosService>();
            services.AddTransient<ITrabajosRepository, TrabajosRepository>();

            services.AddTransient<IReferenciasFinancierasService, ReferenciasFinancierasService>();
            services.AddTransient<IReferenciasFinancierasRepository, ReferenciasFinancierasRepository>();

            services.AddTransient<IReferenciasPersonalesService, ReferenciasPersonalesService>();
            services.AddTransient<IReferenciasPersonalesRepository, ReferenciasPersonalesRepository>();

            services.AddTransient<IReferenciasComercialesService, ReferenciasComercialesService>();
            services.AddTransient<IReferenciasComercialesRepository, ReferenciasComercialesRepository>();

            services.AddTransient<ITelefonoMovilService, TelefonoMovilService>();
            services.AddTransient<ITelefonoMovilRepository, TelefonoMovilRepository>();

            services.AddTransient<IDireccionesService, DireccionesService>();
            services.AddTransient<IDireccionesRepository, DireccionesRepository>();

            services.AddTransient<IMensajesRespuestaRepository, MensajesRespuestaRepository>();

            services.AddTransient<IPersonaNaturalService, PersonaNaturalService>();
            services.AddTransient<IPersonaNaturalRepository, PersonaNaturalRepository>();

            services.AddTransient<ICorreosElectronicosRepository, CorreosElectronicosRepository>();
            services.AddTransient<ICorreosElectronicosService, CorreosElectronicosService>();

            services.AddTransient<IPersonaRepository, PersonaRepository>();
            services.AddTransient<IPersonaService, PersonaService>();

            services.AddTransient<IPersonaNoNaturalRepository, PersonaNoNaturalRepository>();
            services.AddTransient<IPersonaNoNaturalService, PersonaNoNaturalService>();

            services.AddTransient<IAccionistaRepository, AccionistaRepository>();
            services.AddTransient<IAccionistaService, AccionistaService>();

            services.AddTransient<IBienesMueblesRepository, BienesMueblesRepository>();
            services.AddTransient<IBienesMueblesService, BienesMueblesService>();

            services.AddTransient<IBienesInmueblesRepository, BienesInmueblesRepository>();
            services.AddTransient<IBienesInmueblesService, BienesInmueblesService>();

            services.AddTransient<IBienesIntangiblesRepository, BienesIntangiblesRepository>();
            services.AddTransient<IBienesIntangiblesService, BienesIntangiblesService>();

            services.AddTransient<IInformacionAdicionalRepository, InformacionAdicionalRepository>();
            services.AddTransient<IInformacionAdicionalService, InformacionAdicionalService>();

            services.AddTransient<ISriService, SriService>();

            services.AddTransient<IConexion<OracleConnection>, ApiOracleConexion>();

            services.AddSingleton<IPagination, OraclePagination>();

            services.AddTransient<IRelacionInstitucionRepository, RelacionInstitucionRepository>();
            services.AddTransient<IRelacionInstitucionService, RelacionInstitucionService>();
            /*Fin - Inyeccion dependencias Servicios y Repositorios*/

            // Conexion Base de datos
            string connectionString = Configuration.GetSection("ConnectionString").Value;
            services.AddTransient<IDbConnection>((db) => new OracleConnection(connectionString));

            /*Inicio - Configuracion Oracle*/
            var nombreConexion = Configuration.GetValue<string>("ActiveDataBase");
            services.Configure<ConexionDb>(options =>
                Configuration.GetSection("ConnectionObjects:" + nombreConexion).Bind(options));
            services.Configure<ConexionDb>(Configuration.GetSection("OpcionesGenerales"));
            /*Fin - Configuracion Oracle*/

            /*Inicio - Consul*/
            services.AddSingleton<IServiceId, ServiceId>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddConsul();
            /*Fin - Consul*/

            /*Inicio - Fabio*/
            services.AddFabio();
            /*Fin - Fabio*/

            /*Inicio - Tracer distributed*/
            services.AddJaeger();
            services.AddOpenTracing();
            /*Fin - Tracer distributed*/

            /*Inicio - Redis Cache*/
            services.AddRedis();
            services.AddSingleton<IExtensionCache, ExtensionCache>();
            /*Fin - Redis Cache*/


            /*Inicio - Injeccion de clases validadoras*/
            services.AddControllers()
                .AddFluentValidation(cfg =>
                    cfg.RegisterValidatorsFromAssemblyContaining<ObtenerReferenciasComercialesDtoValidador>())
                .AddJsonOptions(options =>
                {
                    // trim en textos
                    options.JsonSerializerOptions.Converters.Add(new TrimStringConverter());
                });
            /*Fin - Injeccion de clases validadoras*/

            /*Inicio - Interceptor para validaciones inputs en ends points con Fluent*/
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            services.AddMvc(options => { options.Filters.Add(typeof(ValidationResultAttribute)); })
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());
            services.AddTransient<IValidatorInterceptor, ValidadorInterceptor>();
            /*Fin - Interceptor de validaciones inputs en ends points Fluent*/

            /*Inicio RabbitMQ*/
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            services.AddRabbitMQ();
            services.AddTransient<IRequestHandler<AuditoriaCommand, bool>, AuditoriaCommandHandler>();
            /*Fin RabbitMQ*/

            services.AddProxyHttp();


            // REGISTRAMOS SWAGGER COMO SERVICIO
            services.AddOpenApiDocument(document =>
            {
                document.Title = "VimaCoop | Personas";
                document.Description = "Endpoints de personas: Administrar CRUD de la información de las personas de la entidad.";
                document.Version = "v1";
               

                // CONFIGURAMOS LA SEGURIDAD JWT PARA SWAGGER,Y AÑADIMOS EL TOKEN JWT A LA CABECERA.
                document.AddSecurity("JWT", Enumerable.Empty<string>(),
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Agregar token."
                    }
                );

                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IHostApplicationLifetime applicationLifetime, IConsulClient consulClient)
        {
            app.UseAuthentication();
            app.UseOpenApi();
            app.UseSwaggerUi3();
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            //Consul
            var serviceId = app.UseConsul();
            applicationLifetime.ApplicationStopped.Register(() => { consulClient.Agent.ServiceDeregister(serviceId); });

            //Seq logs
            app.UseLogSeq();
        }
    }
}