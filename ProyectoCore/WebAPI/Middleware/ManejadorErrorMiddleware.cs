using System;
using System.Net;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebAPI.Middleware
{
    public class ManejadorErrorMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ManejadorErrorMiddleware> logger;
        public ManejadorErrorMiddleware(RequestDelegate _next,ILogger<ManejadorErrorMiddleware> _logger)
        {
            next=_next;
            logger=_logger;
        }
        public async Task Invoke(HttpContext _context)
        {
            try
            {
                  await next(_context);
            }
            catch(Exception ex)
            {
                await ManejadorExcepcionAsincrono(_context,ex,logger);           }
          
        }

        private async Task ManejadorExcepcionAsincrono(HttpContext _context,Exception _ex,ILogger<ManejadorErrorMiddleware> _logger)
        {
            object errores=null;
            switch (_ex)
            {
                case ManejadorExcepcion me:
                    _logger.LogError(_ex,"Manejador de error");
                    errores=me.Errores;
                    _context.Response.StatusCode=(int)me.Codigo;
                    break;
                case Exception e:
                    _logger.LogError(_ex,"Error de servidor");
                    errores =string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                    _context.Response.StatusCode= (int)HttpStatusCode.InternalServerError;
                    break;
            }
            _context.Response.ContentType="application/json";
            if(errores != null){
               var resultados= JsonConvert.SerializeObject(new {errores}); 
               await _context.Response.WriteAsync(resultados);
            }
        }
    }
}