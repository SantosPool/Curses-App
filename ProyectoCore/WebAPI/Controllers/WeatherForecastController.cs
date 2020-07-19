using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Persistencia;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")] //indica dataanotations que sera remplazada por nombre de clase http://localhost/[NombreClase]=>WeatherForecast
    public class WeatherForecastController : ControllerBase
    {
        private readonly CursosOnlineContext context;
        public WeatherForecastController(CursosOnlineContext _context)
        {
            this.context= _context;
        }
        [HttpGet]
        public IEnumerable<Curso> Get()
        {

            // string[] nombres= new[]{"Fabian","Rolando","Rebeca"};
            // return nombres;
            return context.Curso.ToList();
        }
    }
}
