using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Gateway.Api.utilis
{
    public class RequestInterceptor
    {
        private readonly RequestDelegate _next;

        public RequestInterceptor(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string remoteIpAddress;

            if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                remoteIpAddress = httpContext.Request.Headers["X-Forwarded-For"];
            else
                remoteIpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            string hostName = System.Net.Dns.GetHostEntry(remoteIpAddress).HostName.ToString();

            string header = (httpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault() ?? httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault());

            string g = "";
            if (IPAddress.TryParse(header, out IPAddress ip))
            {
                g = ip.ToString();
            }

            Console.WriteLine($"IP PRIVADA- {remoteIpAddress}");
            //Console.WriteLine($"HOSTNAME- {hostName}");
            //Console.WriteLine($"IP PUBLICA- {g}");

            Stream originalResponseBody = httpContext.Response.Body;

            using (var memStream = new MemoryStream())
            {
                //httpContext.Response.Body = memStream;
                //httpContext.Request.Body.Position = 0;
                await _next(httpContext);

                //string bodyResponse = await FormatResponse(memStream);
                ////AppLogger.logInfo($"RESPONSE => {bodyResponse}");

                memStream.Position = 0;
                await memStream.CopyToAsync(originalResponseBody);
            }
        }
    }
}
