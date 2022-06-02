using CoopCrea.Cross.Event.Src.Bus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Personas.Application.Messages.Commands;
using Personas.Application.Utils;
using Personas.Core.App;
using Personas.Core.Interfaces.IRepositories;
using System;

namespace Personas.Application.Middleware
{
    public class FiltroAuditoria : IActionFilter
    {
        private string _endPoint;
        private InformacionToken _infoToken;
        private readonly IEventBus _eventBus;
        protected readonly ConfiguracionApp _config;
        protected readonly ILogsRepository<FiltroAuditoria> _logger;

        public FiltroAuditoria(IEventBus eventBus, ConfiguracionApp config, ILogsRepository<FiltroAuditoria> logger)
        {
            _eventBus = eventBus;
            _config = config;
            _logger = logger;
        }

        public async void OnActionExecuting(ActionExecutingContext context)
        {
            object dto = context.ActionArguments;
            string entrada = System.Text.Json.JsonSerializer.Serialize(dto);

            try
            {
                _infoToken = await JwtAyuda.obtenerInformacioToken(context.HttpContext);
            }
            catch (Exception exc)
            {
                _logger.Error($"========= Error OnActionExecuting al obtener informacion del Token. ======= {exc}");
                context.HttpContext.Response.StatusCode = 401;
                context.HttpContext.Response.Headers.Clear();
                context.Result = new EmptyResult();
                return;
            }

            _endPoint = context.HttpContext.Request.Path;
            _config.guid = Guid.NewGuid().ToString();
            _config.Idioma = _infoToken.idioma;
            _config.Modulo = "personas";
            _config.codigoUsuarioRegistra = _infoToken.codigoUsuario;
            _config.codigoAgencia = _infoToken.codigoAgencia;

            if (HttpMethods.IsGet(context.HttpContext.Request.Method))
            {
                return;
            }

            try
            {
                AuditoriaCommand comando = new AuditoriaCommand()
                {
                    guid = _config.guid,
                    codigoUsuario = _infoToken.codigoUsuario,
                    fechaRegistro = DateTime.Now,
                    modulo = _config.Modulo,
                    endPoint = _endPoint,
                    entrada = entrada,
                    codigoSucursal = _infoToken.codigoSucursal,
                    codigoAgencia = _infoToken.codigoAgencia,
                    ipPublica = _infoToken.ipPublica,
                    ipPrivada = _infoToken.ipPrivada,
                    navegador = _infoToken.navegador
                };

                await _eventBus.SendCommand(comando);
            }
            catch (Exception exc)
            {
                _logger.Error($"========= Error filtro OnActionExecuting al enviar objeto a RABBITMQ. ======= {exc}");
            }

        }

        public async void OnActionExecuted(ActionExecutedContext context)
        {
            string response = null;

            if (context.Result != null)
            {
                string jsonString = JsonConvert.SerializeObject(context.Result);
                JObject rest = JsonConvert.DeserializeObject<dynamic>(jsonString);
                if (rest["Value"] != null)
                {
                    response = rest["Value"].ToString();
                }
            }

            if (HttpMethods.IsGet(context.HttpContext.Request.Method))
            {
                return;
            }
            AuditoriaCommand comando = null;

            try
            {
                comando = new AuditoriaCommand()
                {
                    guid = _config.guid,
                    codigoUsuario = _infoToken.codigoUsuario,
                    fechaRegistro = DateTime.Now,
                    modulo = _config.Modulo,
                    endPoint = _endPoint,
                    salida = response,
                    codigoSucursal = _infoToken.codigoSucursal,
                    codigoAgencia = _infoToken.codigoAgencia,
                    ipPublica = _infoToken.ipPublica,
                    ipPrivada = _infoToken.ipPrivada,
                    navegador = _infoToken.navegador
                };
                await _eventBus.SendCommand(comando);

            }
            catch (Exception exc)
            {
                _logger.Error($"========= Error filtro OnActionExecuted al enviar objeto a RABBITMQ. ======= {exc}");
            }

        }

    }
}
